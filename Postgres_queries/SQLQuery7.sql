DROP TABLE IF EXISTS HomePageData
CREATE TABLE HomePageData (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255),
    Title NVARCHAR(255),
    Subtitle NVARCHAR(255),
    EmailContacts NVARCHAR(MAX),
    FooterHeading NVARCHAR(MAX),
    FooterText NVARCHAR(MAX),
    CreatedDate DATETIME,
    LMDT DATETIME
);

DROP TABLE IF EXISTS HeroCarouselItem
CREATE TABLE HeroCarouselItem (
    Id INT PRIMARY KEY IDENTITY(1,1),
    HomePageDataId INT,
	ImgSize INT,
    Heading NVARCHAR(255),
	Detail NVARCHAR(255),
    ImageUrl NVARCHAR(255),
    FOREIGN KEY (HomePageDataId) REFERENCES HomePageData(Id) ON DELETE CASCADE
);