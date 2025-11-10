# Marketing Campaign Analyzer

Hello Visitors!! This is my marketing campaign management project that I built for analyzing leads and campaigns. Its basically a web app where different users can manage their own marketing data separately along with their campaigns

## What this project does

- Users can create and manage marketing campaigns
- Add leads to campaigns with auto segment assignment
- Upload bulk leads from CSV files
- View analytics dashboard with charts
- Track campaign performance over time
- Each user only sees their own data (Multiple User Dashboard support)

## Tech stuff I used

**Backend:**
- ASP.NET Core Web API Version 8.0
- Entity Framework Core
- SQL Server database
- JWT authentication

**Frontend:**
- Angular 20
- Chart.js for graphs
- Papa Parse for csv parsing

## Database Setup

You need MySQL Workbench CE running on your machine. Database Name: MarketingCampaignDB
### Connection String

Go to `server/MarketingCampaignServer/MarketingCampaignServer/appsettings.json` and change this line:

`"DefaultConnection": "Server=localhost;Database=MarketingCampaignDB;User Id=username;Password=password;TrustServerCertificate=true;"`

### Database Creation
Please Follow the Query File to set up the database and table in your db.
Sql Scripts.sql file -- File Name

###  Application Db Context and Entities creation automatically as per db in server side
dotnet ef dbcontext scaffold "Server=localhost;Port=3306;Database=marketing_campaign;User=root;Password=Kanhapreet@88;" Pomelo.EntityFrameworkCore.MySql --output-dir Models/Entities --context-dir Data --context ApplicationDbContext --use-database-names --force
Run this command to get automated entities and ApplicationDb context creation in the code as db first approach

## How to run this thing

### Backend (API)

1. Open the `server` folder in Visual Studio 
2. Make sure you have .NET 8 SDK installed
3. Change the connection string in appsettings.json if needed
4. Run on the Profiles you have selected and it will build automatically and run the project on desired profile.
5. API will start on the profile setted

### Frontend (Angular)

1. Open the `client/client-app` folder in VS Code
2. Make sure you have Node.js installed.
3. Install Angular CLI if you don't have it: `npm install -g @angular/cli`
4. Install dependencies: `npm i`
5. Start the app: `ng serve`
6. Open browser and go to localhost:4200

## Features explained

### User Management
- Each user has their own data completely separated
- JWT tokens with OTP verification for security
- Session timeout after inactivity

### Campaign Management
- Create campaigns with start/end dates
- Metrics get calculated automatically from leads data
- Can assign leads to specific campaigns

### Lead Management
- Add leads manually or bulk upload from CSV
- Auto segment assignment based on email domain,campaign and phone number
- Segments like "India Leads", "Corporate", "General" etc
- Search and filter leads by campaign or segment, multilead search based on name and email-id.

### Bulk Upload
- Upload CSV files with leads data
- Preview before uploading
- Shows success/error count after upload
- Download template CSV file

### Analytics Dashboard
- Bar charts showing campaign performance
- Pie chart for lead segment distribution 
- Export campaign data to CSV
- All charts update automatically when data changes after reevaluation

### Responsive Design
- Works on mobile and desktop
- Basic CSS styling
- Simple colors and layouts

## Common Issues

**Database connection fails:**
- Check if SQL Server is running
- Verify connection string is correct
- Make sure database user has proper permissions

**Angular build errors:**
- Delete node_modules folder and run `npm install` again
- Check if you have the right Node.js version

**CORS errors:**
- Backend has CORS enabled for localhost:4200
- If you change frontend port, update CORS settings in Program.cs

## Future improvements I might add

- Email notifications for campaigns to calcullate the actual engagement metrics
- More chart types in analytics
- Lead scoring system
- Better mobile UI
- Dark mode maybe added as per the requiremnt 
- Campaign Calender can be added to show all campaigns active on a month.

## Notes

- This is a demo project so code might not be perfect
- CSS is basic with normal modals, grid, flex and div styling
- Some features might have bugs, feel free to fix them
- Database grows over time, might need cleanup scripts later

That's pretty much it! The app should work fine for managing marketing campaigns and leads.