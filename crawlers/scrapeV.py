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
    article_text = soup.find("h1", class_="title2 text-blue-100 mt-6").text
    article_text += ". "

    #Subtitle
    article_text += soup.find("div", class_="richText").text
    
    #Body
    article = soup.find("div", class_="richText neos-nodetypes-text")
    if soup.find("div", class_="richText neos-nodetypes-text"):
        article = soup.find("div", class_="richText neos-nodetypes-text")
    else: 
        article = soup.find("div", class_="richText")
    for p in article.find_all("p"):
        article_text += p.text  
    
    return article_text

def getArticles(driver, page_num, articles):
    html = driver.page_source
    soup = BeautifulSoup(html, "html.parser")

    skipped_articles = 0

    #Getting the links to articles
    for tag in soup.find_all("a", class_="focus:outline-none"):
        articles += 1
        if tag["href"] == "https://www.venstre.dk/nyheder/sommergruppemode-det-skal-kunne-betale-sig-at-arbejde":
            continue
        else: 
            driver.get(tag["href"])

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
        
        f.close

        print("{}: ".format(articles) + tag["href"])

        if articles == 100:
            break

    if (articles < 100): 
        page_num += 1

        #Next page
        driver.get("https://www.venstre.dk/nyheder/seneste-nyheder?page={}".format(page_num))

        getArticles(driver, page_num, articles)

#Initial page
driver.get("https://www.venstre.dk/nyheder/seneste-nyheder")
WebDriverWait(driver, 10).until(EC.element_to_be_clickable((By.CLASS_NAME, "coi-banner__accept"))).click()

getArticles(driver, page_num, articles)