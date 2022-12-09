import time
import os
from bs4 import BeautifulSoup
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.firefox.service import Service

abs_path = os.path.dirname(__file__)
rel_path = r"lib\\geckodriver.exe"

option = webdriver.FirefoxOptions()
option.headless = True
option.binary_location = r"C:\\Program Files\\Mozilla Firefox\\firefox.exe"
driverService = Service(os.path.join(abs_path, rel_path))
driver = webdriver.Firefox(service=driverService, options=option)

articles = 0
page_num = 1  

def scrapeArticle(driver):
    html = driver.page_source
    soup = BeautifulSoup(html, "html.parser")

    article_text = ""
    
    #Title    
    if soup.find("h1", class_="o-headline-1--big inline"):
        article_text = soup.find("h1", class_="o-headline-1--big inline").text
    else:
        article_text = soup.find("h1", class_="o-headline-1 inline").text
    article_text += ". "

    #subheader
    article_text += soup.find("div", class_="leading-130").text

    #Body
    if soup.find("div", class_="s-rte"):
        for div in soup.find_all("div", class_="s-rte"):
            for p in div.find_all("p"):
                article_text += p.text + " "

    return article_text

def getArticles(driver, page_num, articles):
    print(driver.current_url)

    for i in range(10):
        driver.execute_script("window.scrollTo(0, document.body.scrollHeight);")
        WebDriverWait(driver, 10).until(EC.element_to_be_clickable((By.XPATH, "/html/body/div[2]/div/div/div[3]/main/section/div/div[3]/div[5]/button"))).click()
        time.sleep(0.5)
        
    html = driver.page_source
    soup = BeautifulSoup(html, "html.parser")

    skipped_articles = 0

    #Getting the links to articles
    for tag in soup.find_all("a", class_="spa-link uppercase hover:text-pink leading-100 cursor-pointer spa-link--prefetched"):
        articles += 1
        driver.get("https://www.radikale.dk" + tag["href"])

        file_length = ""

        #Saving the articles
        if (articles < 10):
            with open("articles\\0{}.txt".format(articles), "w", encoding="utf-8") as f:
                f.write(scrapeArticle(driver))
            with open("articles\\0{}.txt".format(articles), "r", encoding="utf-8") as f:
                file = f.read()
                file_length = file.split()
                if len(file_length) < 100:
                    print("Skipping")
                    articles -= 1
                    skipped_articles += 1

        else:
            with open("articles\\{}.txt".format(articles), "w", encoding="utf-8") as f:
                f.write(scrapeArticle(driver))
            with open("articles\\{}.txt".format(articles), "r", encoding="utf-8") as f:
                file = f.read()
                file_length = file.split()
                if len(file_length) < 100:
                    print("Skipping")
                    articles -= 1
                    skipped_articles += 1

        print("{}: ".format(articles) + tag["href"])

        if articles == 100:
            break

    print("Skipped: {} articles".format(skipped_articles))

#Initial page with articles
driver.get("https://www.radikale.dk/aktuelt/?typeId=-1&offset=90&limit=9")

#Cookie notice
WebDriverWait(driver, 10).until(EC.element_to_be_clickable((By.CLASS_NAME, "coi-banner__accept"))).click()

getArticles(driver, page_num, articles)