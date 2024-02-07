USE master;  
GO  
CREATE DATABASE troj3  
ON   
( NAME = troj3,  
    FILENAME = 'D:/repo/TrojApp/Database/Data/troj3.mdf',  
    SIZE = 10,  
    MAXSIZE = 50,  
    FILEGROWTH = 5 )  
LOG ON  
( NAME = troj3_log,  
    FILENAME = 'D:/repo/TrojApp/Database/Data/troj3log.ldf',  
    SIZE = 5MB,  
    MAXSIZE = 25MB,  
    FILEGROWTH = 5MB );  
GO 