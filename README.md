Application Run and Debug Procedure:

1. After downloading this project, open this project with VS2022
2. You need dot Net 6 framework for run and debug this project
3. Change database connection Appsetting.JSON file as per your SQL server setup

Your need to change following setings in appsettings.json

1. SqlConnectionString": "Data Source=(localdb)\\MSSQLLocalDB;MultipleActiveResultSets=true;Initial Catalog=MedicineDispatchDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
====================================================

DBA Setup procedure:
1. First create database named "MedicineDispatchDB"
2. In Application Solution there is a file named "GeneratedSchemaScirpt" where you can find the script for tables and store procedure accordingly.
3. There is also a script file for sample data insertion named "SampleDataInsertScript"
