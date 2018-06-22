Update Country SET CountryName=REPLACE(CountryName,CHAR(10),'')
Update Country SET CountryName=REPLACE(CountryName,CHAR(13),'')

Update [State] SET StateName=REPLACE(StateName,CHAR(10),'')
Update [State] SET StateName=REPLACE(StateName,CHAR(13),'')


Update City SET [Name]=REPLACE([Name],CHAR(13),'')
Update City SET [Name]=REPLACE([Name],CHAR(10),'')