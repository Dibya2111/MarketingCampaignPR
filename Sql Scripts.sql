USE marketing_campaign;

CREATE TABLE Users (
  UserId BIGINT AUTO_INCREMENT PRIMARY KEY,
  Username VARCHAR(100) NOT NULL,
  PasswordHash VARCHAR(255) NOT NULL,
  Role VARCHAR(50) DEFAULT 'User',
  CreatedByUserId BIGINT NULL,
  CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
  LastModifiedUserId BIGINT NULL,
  LastModifiedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  IsActive BOOLEAN DEFAULT TRUE,
  IsDeleted BOOLEAN DEFAULT FALSE
);

CREATE TABLE Campaigns (
  CampaignId BIGINT AUTO_INCREMENT PRIMARY KEY,
  CampaignName VARCHAR(100) NOT NULL,
  StartDate DATE,
  EndDate DATE,
  TotalLeads INT DEFAULT 0,
  OpenRate DECIMAL(5,2) DEFAULT 0.00,
  ConversionRate DECIMAL(5,2) DEFAULT 0.00,
  Status VARCHAR(50) DEFAULT 'Active',
  CreatedByUserId BIGINT NULL,
  CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
  LastModifiedUserId BIGINT NULL,
  LastModifiedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  IsActive BOOLEAN DEFAULT TRUE,
  IsDeleted BOOLEAN DEFAULT FALSE,
  FOREIGN KEY (CreatedByUserId) REFERENCES Users(UserId),
  FOREIGN KEY (LastModifiedUserId) REFERENCES Users(UserId)
);

CREATE TABLE Leads (
  LeadId BIGINT AUTO_INCREMENT PRIMARY KEY,
  Name VARCHAR(100) NOT NULL,
  Email VARCHAR(150) NOT NULL UNIQUE,
  Phone VARCHAR(20),
  CampaignId BIGINT,
  Segment VARCHAR(50) DEFAULT 'General',
  CreatedByUserId BIGINT NULL,
  CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
  LastModifiedUserId BIGINT NULL,
  LastModifiedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  IsActive BOOLEAN DEFAULT TRUE,
  IsDeleted BOOLEAN DEFAULT FALSE,
  FOREIGN KEY (CampaignId) REFERENCES Campaigns(CampaignId) ON DELETE SET NULL,
  FOREIGN KEY (CreatedByUserId) REFERENCES Users(UserId),
  FOREIGN KEY (LastModifiedUserId) REFERENCES Users(UserId)
);

CREATE TABLE EngagementMetrics (
  MetricId BIGINT AUTO_INCREMENT PRIMARY KEY,
  LeadId BIGINT,
  OpenRate DECIMAL(5,2) DEFAULT 0.00,
  ClickRate DECIMAL(5,2) DEFAULT 0.00,
  ConversionRate DECIMAL(5,2) DEFAULT 0.00,
  CreatedByUserId BIGINT NULL,
  CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
  LastModifiedUserId BIGINT NULL,
  LastModifiedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  IsActive BOOLEAN DEFAULT TRUE,
  IsDeleted BOOLEAN DEFAULT FALSE,
  FOREIGN KEY (LeadId) REFERENCES Leads(LeadId) ON DELETE CASCADE,
  FOREIGN KEY (CreatedByUserId) REFERENCES Users(UserId),
  FOREIGN KEY (LastModifiedUserId) REFERENCES Users(UserId)
);

CREATE TABLE BulkUploadLogs (
  UploadId BIGINT AUTO_INCREMENT PRIMARY KEY,
  UploadedBy BIGINT,
  UploadedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
  TotalRecords INT DEFAULT 0,
  ValidRecords INT DEFAULT 0,
  InvalidRecords INT DEFAULT 0,
  CreatedByUserId BIGINT NULL,
  CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
  LastModifiedUserId BIGINT NULL,
  LastModifiedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  IsActive BOOLEAN DEFAULT TRUE,
  IsDeleted BOOLEAN DEFAULT FALSE,
  FOREIGN KEY (UploadedBy) REFERENCES Users(UserId),
  FOREIGN KEY (CreatedByUserId) REFERENCES Users(UserId),
  FOREIGN KEY (LastModifiedUserId) REFERENCES Users(UserId)
);

CREATE TABLE CampaignSegments (
  SegmentId BIGINT AUTO_INCREMENT PRIMARY KEY,
  SegmentName VARCHAR(100) NOT NULL,
  Description VARCHAR(255),
  CreatedByUserId BIGINT,
  CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
  LastModifiedUserId BIGINT,
  LastModifiedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  IsActive BOOLEAN DEFAULT TRUE,
  IsDeleted BOOLEAN DEFAULT FALSE,
  FOREIGN KEY (CreatedByUserId) REFERENCES Users(UserId),
  FOREIGN KEY (LastModifiedUserId) REFERENCES Users(UserId)
);

CREATE TABLE BulkUploadDetails (
  DetailId BIGINT AUTO_INCREMENT PRIMARY KEY,
  UploadId BIGINT,
  LeadEmail VARCHAR(150),
  ValidationStatus VARCHAR(50), -- e.g., Valid / Duplicate / Invalid
  Message VARCHAR(255),
  CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (UploadId) REFERENCES BulkUploadLogs(UploadId)
);

CREATE TABLE CampaignPerformanceSnapshots (
  SnapshotId BIGINT AUTO_INCREMENT PRIMARY KEY,
  CampaignId BIGINT,
  DateCaptured DATETIME DEFAULT CURRENT_TIMESTAMP,
  TotalLeads INT,
  OpenRate DECIMAL(5,2),
  ConversionRate DECIMAL(5,2),
  FOREIGN KEY (CampaignId) REFERENCES Campaigns(CampaignId)
);

INSERT INTO CampaignSegments (SegmentName, Description, CreatedDate, IsActive)
VALUES
('Seasonal', 'Leads from summer or holiday campaigns', NOW(), TRUE),
('Corporate', 'Leads related to corporate or enterprise offers', NOW(), TRUE),
('Early Adopters', 'Leads for new product launch campaigns', NOW(), TRUE),
('Corporate Leads', 'Email domain ends with @company.com', NOW(), TRUE),
('Student/Academic', 'Email domain ends with @edu.org', NOW(), TRUE),
('General Public', 'Email domain ends with @gmail.com or @yahoo.com', NOW(), TRUE),
('US Leads', 'Phone numbers starting with +1 country code', NOW(), TRUE),
('India Leads', 'Phone numbers starting with +91 country code', NOW(), TRUE),
('General', 'Default segment when no other rule applies', NOW(), TRUE);


CREATE TABLE Agencies (
  AgencyId BIGINT AUTO_INCREMENT PRIMARY KEY,
  AgencyName VARCHAR(100),
  CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
  IsActive BOOLEAN DEFAULT TRUE
);

CREATE TABLE Buyers (
  BuyerId BIGINT AUTO_INCREMENT PRIMARY KEY,
  BuyerName VARCHAR(100),
  CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
  IsActive BOOLEAN DEFAULT TRUE
);

CREATE TABLE Brands (
  BrandId BIGINT AUTO_INCREMENT PRIMARY KEY,
  BrandName VARCHAR(100),
  CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
  IsActive BOOLEAN DEFAULT TRUE
);

ALTER TABLE engagementmetrics 
ADD COLUMN CampaignId BIGINT NULL,
ADD CONSTRAINT fk_campaign FOREIGN KEY (CampaignId) REFERENCES campaigns(CampaignId);

ALTER TABLE engagementmetrics
ADD COLUMN EmailsSent INT DEFAULT 0 AFTER LeadId,
ADD COLUMN EmailsOpened INT DEFAULT 0 AFTER EmailsSent,
ADD COLUMN Clicks INT DEFAULT 0 AFTER EmailsOpened,
ADD COLUMN Conversions INT DEFAULT 0 AFTER Clicks;

ALTER TABLE Campaigns
ADD COLUMN BuyerId BIGINT NULL,
ADD COLUMN AgencyId BIGINT NULL,
ADD COLUMN BrandId BIGINT NULL,
ADD CONSTRAINT FK_Campaigns_Buyer FOREIGN KEY (BuyerId) REFERENCES Buyers(BuyerId),
ADD CONSTRAINT FK_Campaigns_Agency FOREIGN KEY (AgencyId) REFERENCES Agencies(AgencyId),
ADD CONSTRAINT FK_Campaigns_Brand FOREIGN KEY (BrandId) REFERENCES Brands(BrandId);

INSERT INTO buyers (BuyerName, CreatedDate, IsActive) VALUES
('Tata Consultancy Services (TCS)', NOW(), TRUE),
('Infosys Limited', NOW(), TRUE),
('Wipro Technologies', NOW(), TRUE),
('HDFC Bank', NOW(), TRUE),
('ICICI Bank', NOW(), TRUE),
('Reliance Industries', NOW(), TRUE),
('Bharti Airtel', NOW(), TRUE),
('Hindustan Unilever (HUL)', NOW(), TRUE),
('Mahindra & Mahindra', NOW(), TRUE),
('Aditya Birla Group', NOW(), TRUE);

INSERT INTO agencies (AgencyName, CreatedDate, IsActive) VALUES
('Dentsu Creative India', NOW(), TRUE),
('Ogilvy India', NOW(), TRUE),
('Madison World', NOW(), TRUE),
('Leo Burnett India', NOW(), TRUE),
('McCann Worldgroup India', NOW(), TRUE),
('VMLY&R India', NOW(), TRUE),
('Interactive Avenues', NOW(), TRUE),
('Publicis Groupe India', NOW(), TRUE),
('FoxyMoron', NOW(), TRUE),
('Social Panga', NOW(), TRUE);

INSERT INTO brands (BrandName, CreatedDate, IsActive) VALUES
('Amul', NOW(), TRUE),
('Parle-G', NOW(), TRUE),
('Swiggy', NOW(), TRUE),
('Zomato', NOW(), TRUE),
('Ola Cabs', NOW(), TRUE),
('Flipkart', NOW(), TRUE),
('Big Bazaar', NOW(), TRUE),
('Apollo Tyres', NOW(), TRUE),
('Bajaj Auto', NOW(), TRUE),
('Godrej Consumer Products', NOW(), TRUE);

CREATE TABLE OtpLogins (
  OtpId BIGINT AUTO_INCREMENT PRIMARY KEY,
  UserId BIGINT NOT NULL,
  OtpCode VARCHAR(6) NOT NULL,
  GeneratedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
  ExpirationTime DATETIME NOT NULL,
  IsVerified BOOLEAN DEFAULT FALSE,
  AttemptCount INT DEFAULT 0,
  CreatedByUserId BIGINT NULL,
  CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
  LastModifiedUserId BIGINT NULL,
  LastModifiedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  IsActive BOOLEAN DEFAULT TRUE,
  IsDeleted BOOLEAN DEFAULT FALSE,
  FOREIGN KEY (UserId) REFERENCES Users(UserId),
  FOREIGN KEY (CreatedByUserId) REFERENCES Users(UserId),
  FOREIGN KEY (LastModifiedUserId) REFERENCES Users(UserId)
);

