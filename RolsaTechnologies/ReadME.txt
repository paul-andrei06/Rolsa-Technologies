# ASP.NET MVC Website - Setup & Credentials

## Prerequisites  
Ensure you have **Visual Studio 2022** installed suitable for .NET development.  

## Database Migration  
To update the database, follow these steps:  

1. Open **Visual Studio 2022**.  
2. Navigate to **Tools** > **NuGet Package Manager** > **Package Manager Console**.  
3. In the **Package Manager Console**, run the following command:  
   ```
   Update-Database
   ```  
4. Once the command executes successfully, the database is updated.  



## Login Credentials  

### Admin User  
- **Email:** `admin@example.com`  
- **Password:** `Password123@`  

### Regular Users  
- **Option 1:**  
  - **Email:** `user@example.com`  
  - **Password:** `Password123@`  

- **Option 2:**
  -**Email:** `user2@example.com`
  -**Password:** `Password123@`

### Professional
- **Email:** `professional@example.com`
- **Password:** `Password123@`




## Notes  
- Ensure your database connection string is correctly configured in `appsettings.json` or `web.config`.  
- If the database update fails, check if Entity Framework is properly installed and migrations are applied.  

For further assistance, refer to the project documentation

