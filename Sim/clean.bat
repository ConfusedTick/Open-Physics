@echo off
setlocal
cd /d %~dp0
rmdir /s /q .vs
rmdir /s /q bin
rmdir /s /q obj