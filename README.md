# FCS-Analysis

In order to run this application, you have to install external packages using NuGet package manager.

This repository contains 2 projects - Windows version and Online version.

The two projects are using same FCMeasurement module.


1. Windows Version.
   
   In executable path, there have to be 
   
   results.txt    :     The results of Mie Scattering Calculation, i.e. V,HC : S1, S2 calculation results.
   
   
   It contains 3 Forms as 3 tabs - WBC, RBC, Mie Scatter. It's a combination 3 projects.
   
   
2. Online Version.

    This is ASP.NET Core project using Mysql Entity Framework.
    
    In the appsettings.json, there is mysql connection string, which is used to connect to mysql server.
    
    At the start of application, it ensures whether there is created db already. If not, it will create a new db from ApplicationDbContext. So that there is no manual setup for db.
    When Creating new db, it will seed initial data into table.
    
    The Admin Credential is email : admin@gmail.com, password : secret.
    
    
    
    
