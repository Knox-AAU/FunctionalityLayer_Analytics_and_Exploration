import time
import re
from bs4 import BeautifulSoup
from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.firefox.service import Service

option = webdriver.FirefoxOptions()
#option.headless = True
option.binary_location = r"C:\\Program Files\\Mozilla Firefox\\firefox.exe"
driverService = Service(r"C:\\Users\Ander\\crawler\\lib\\geckodriver.exe")
driver = webdriver.Firefox(service=driverService, options=option)

articles = 0
page_num = 1  

def scrapeArticle(driver):
    html = driver.page_source
    soup = BeautifulSoup(html, "html.parser")

    article_text = ""
    
    #Title
    article_text = soup.find("", class_="").text
    article_text += ". "

    #driver.execute_script("window.scrollTo(0, document.body.scrollHeight);")

    #Subtitle
    article_text += soup.find("", class_="").text

    #Body
    article = soup.find("", class_="")
    for p in article.find_all(""):
        article_text += p.text  
    
    return article_text

def getArticles(driver, page_num, articles):
    print(driver.current_url)

    html = driver.page_source
    soup = BeautifulSoup(html, "html.parser")

    skipped_articles = 0

    #Getting the links to articles
    for tag in soup.find_all("", class_=""):
        articles += 1
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
        driver.get("{}".format(page_num))

        getArticles(driver, page_num, articles)

#Initial page with articles
driver.get("")

#Cookie notice
WebDriverWait(driver, 10).until(EC.element_to_be_clickable((By.CLASS_NAME, ""))).click()

getArticles(driver, page_num, articles)