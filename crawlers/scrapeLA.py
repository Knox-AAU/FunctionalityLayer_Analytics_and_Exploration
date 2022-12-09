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
    article_text = soup.find("h1", class_="m-0").text
    article_text += ". "

    #Body
    for section in soup.find_all("section", class_="content-section content-section-wysiwyg wysiwyg-content"):
        for p in section.find_all("p"):
            article_text += p.text + " "
    
    return article_text

def getArticles(driver, page_num, articles):
    print(driver.current_url)

    html = driver.page_source
    soup = BeautifulSoup(html, "html.parser")

    #Getting links to articles
    for tag in soup.find_all("a", class_="grid-item-inner"):
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

        print("{}: ".format(articles) + tag["href"])

        if articles == 100:
            break

    if (articles < 100): 
        page_num += 1

        #Next page
        driver.get("https://www.liberalalliance.dk/nyheder/?_post_types=debat&_paged={}".format(page_num))

        getArticles(driver, page_num, articles)

#Initial page
driver.get("https://www.liberalalliance.dk/nyheder/?_post_types=debat")
WebDriverWait(driver, 10).until(EC.element_to_be_clickable((By.CLASS_NAME, "coi-banner__accept"))).click()

getArticles(driver, page_num, articles)