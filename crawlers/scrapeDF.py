import os
import time
from bs4 import BeautifulSoup
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.firefox.service import Service

abs_path = os.path.dirname(__file__)
rel_path = r"lib\\geckodriver.exe"

option = webdriver.FirefoxOptions()
option.headless = False
option.binary_location = r"C:\\Program Files\\Mozilla Firefox\\firefox.exe"
driverService = Service(os.path.join(abs_path, rel_path))
driver = webdriver.Firefox(service=driverService, options=option)

def scrapeArticlePia(driver):
    html = driver.page_source
    soup = BeautifulSoup(html, "html.parser")

    article_text = ""
    
    #Title
    article_text = soup.find("h1", class_="c-article-top-info__title").text
    article_text += ". "

    #subheader
    article_text += soup.find("p", class_="c-article-top-info__description").text + " "

    #Body
    text_bodies = soup.find_all("div", class_="c-article-inline-element-container__text-width")
    text_bodies.pop() #Every Jyllands Posten article ends in a contact reminder
    for paragraph in text_bodies:
        for p in paragraph.find_all("p"):
            article_text += p.text + " "
    
    return article_text

def scrapeArticleMorten(driver):
    driver.execute_script("window.scrollTo(0, document.body.scrollHeight);")
    html = driver.page_source
    soup = BeautifulSoup(html, "html.parser")

    article = soup.find("div", class_="fusion-title title fusion-title-2 fusion-sep-none fusion-title-center fusion-title-text fusion-title-size-one")

    article_text = ""
    
    #Title
    article_text = article.find("h1", class_="title-heading-center").text
    article_text += ". "

    #Body
    article = soup.find("div", class_="fusion-text fusion-text-1")
    for p in article.find_all("p"):
        article_text += p.text + " "
    
    return article_text

def saveArticle(article, article_num):
    #Saving the articles
        if (article_num < 10):
            with open("{}\\articles\\0{}.txt".format(abs_path, article_num), "w", encoding="utf-8") as f:
                f.write(article)

        else:
            with open("{}\\articles\\0{}.txt".format(abs_path, article_num), "w", encoding="utf-8") as f:
                f.write(article)
        
        f.close

def findArticles(driver, last_articles_found):
    time.sleep(1)
    html = driver.page_source
    soup = BeautifulSoup(html, "html.parser")

    #Scrolling the page to reveal new articles
    driver.execute_script("window.scrollTo(0, document.body.scrollHeight);")
    time.sleep(1)
    articles_found = soup.find_all("div", class_="content")

    if (articles_found != last_articles_found):
        findArticles(driver, articles_found) #If new articles were found another iteration is needed
    else: 
        #No new articles found so scraping can begin
        print("Found {} articles".format(len(articles_found)))
        article_num = 0

        for article in articles_found:
            article_num += 1
            link = article.find("a", class_="post-link-btn")
            driver.get(link["href"])

            print("Getting: "+ link["href"])

            if (article.find("p", class_="post-category-timeline").text == "Mortens Nyhedsbrev "):
                print("Getting Morten: {} -|- {}".format(article.find("h4", attrs={"class": None}).text, link["href"]))
                saveArticle(scrapeArticleMorten(driver), article_num)
            elif (article.find("p", class_="post-category-timeline").text == "Pias Blog "):
                print("Getting Pia: {} -|- {}".format(article.find("h4", attrs={"class": None}).text, link["href"]))
                saveArticle(scrapeArticlePia(driver), article_num)


def readyPage(driver):
    
    WebDriverWait(driver, 10).until(EC.element_to_be_clickable((By.XPATH, "/html/body/div[1]/div/main/div/section/div/div/div/div/div/div/div/div/div/div[2]/div/div[1]/div[1]/div/div[4]/label[1]/span"))).click()
    driver.find_element(By.XPATH, "/html/body/div[1]/div/main/div/section/div/div/div/div/div/div/div/div/div/div[2]/div/div[1]/div[1]/div/div[5]/label[1]/span").click()
    driver.find_element(By.XPATH, "/html/body/div[1]/div/main/div/section/div/div/div/div/div/div/div/div/div/div[2]/div/div[1]/div[2]/a[2]").click()

    findArticles(driver, "")

#Removing cookie notice immidiately from JP before accessing articles
driver.get("https://jyllands-posten.dk/")
WebDriverWait(driver, 10).until(EC.element_to_be_clickable((By.CLASS_NAME, "CybotCookiebotDialogBodyButton"))).click()

#Where all of DF's news articles are archived
driver.get("https://danskfolkeparti.dk/nyheder/")

readyPage(driver)