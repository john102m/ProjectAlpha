Declare @Id INT = 3

SELECT * FROM PageData pd
    LEFT JOIN BulletPoint bp ON pd.Id = bp.PageDataId
    WHERE pd.Id in (1,2,3)