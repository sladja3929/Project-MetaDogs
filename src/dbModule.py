host = '203.250.148.33'
port = 20306
user = 'root'
passwd = 'Mdogs*nklk*'
userdb = 'user'
nftdb = 'nft'
logdb = 'log'
charset = 'utf8'

import pymysql

class Database():
    def __init__(self, dbname):
        self.db = pymysql.connect(host = host,
                                  port = port,
                                  user = user,
                                  password=passwd,
                                  db=dbname,
                                  charset=charset)
        self.cursor = self.db.cursor(pymysql.cursors.DictCursor)
 
    def execute(self, query, args={}):
        self.cursor.execute(query, args)  
 
    def executeOne(self, query, args={}):
        self.cursor.execute(query, args)
        row = self.cursor.fetchone()
        return row
 
    def executeAll(self, query, args={}):
        self.cursor.execute(query, args)
        row = self.cursor.fetchall()
        return row
    
    def commit(self):
        self.db.commit()

