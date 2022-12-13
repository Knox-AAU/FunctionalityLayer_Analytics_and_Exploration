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
    if soup.find("div", class_="col-xs-12 col-sm-8"):
        article = soup.find("div", class_="col-xs-12 col-sm-8")
    else:
        article = soup.find("div", class_="col-xs-12")
    
    article_text = article.find("h1", attrs={"class": None}).text
    article_text += ". "

    #Body
    index = 0
    for p in article.find_all("p"):
        if index == 0:
            index += 1
            continue
        article_text += p.text + " "
    
    return article_text

def getArticles(driver, page_num, articles):
    print(driver.current_url)

    html = driver.page_source
    soup = BeautifulSoup(html, "html.parser")

    #Getting the links to articles
    for tag in soup.find_all("article", class_="clearfix"):
        link = tag.find("a", attrs={'class':None})
        articles += 1
        driver.get(link["href"])

        print("Getting: "+ link["href"])

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

        else:
            with open("articles\\{}.txt".format(articles), "w", encoding="utf-8") as f:
                f.write(scrapeArticle(driver))
            with open("articles\\{}.txt".format(articles), "r", encoding="utf-8") as f:
                file = f.read()
                file_length = file.split()
                if len(file_length) < 100:
                    print("Skipping")
                    articles -= 1
        
        f.close

        if articles == 100:
            break

    if (articles < 100): 
        page_num += 1
        #Next page
        driver.get("https://sf.dk/nyheder/side/{}/".format(page_num))

        getArticles(driver, page_num, articles)

#Initial page with articles
driver.get("https://sf.dk/nyheder/")

#Cookie notice
WebDriverWait(driver, 10).until(EC.element_to_be_clickable((By.CLASS_NAME, "CybotCookiebotDialogBodyButton"))).click()

getArticles(driver, page_num, articles)