@echo off
rmdir /q /s C:\Users\tybik\source\repos\backup\
xcopy /s C:\Users\tybik\Documents\GitHub\Open-Physics\ C:\Users\tybik\source\repos\backup
@echo Cloned
pause