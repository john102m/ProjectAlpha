DROP TABLE IF EXISTS PageData
CREATE TABLE PageData (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255),
    Title NVARCHAR(255),
    Subtitle NVARCHAR(255),
    TitleImageUrl  NVARCHAR(255),
    BannerImageUrl  NVARCHAR(255),
	Icon NVARCHAR(255),
    DetailTitle NVARCHAR(255),
    DetailHeading NVARCHAR(255),
    DetailSummary NVARCHAR(MAX),
    DetailDescription NVARCHAR(MAX),
    BulletPoints NVARCHAR(MAX),
    CarouselImageUrls NVARCHAR(MAX),
    CreatedDate DATETIME,
    LastModifiedDate DATETIME
);