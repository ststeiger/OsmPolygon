-- https://maps.googleapis.com/maps/api/geocode/json?address=Bern Helvetiastrasse 37

SELECT 
    T_AP_Standort.SO_UID
   ,T_AP_Gebaeude.GB_UID
   ,T_AP_Gebaeude.GB_Nr
   ,T_AP_Gebaeude.GB_Bezeichnung
    
   --,T_AP_Gebaeude.GB_Strasse
   --,T_AP_Gebaeude.GB_StrasseNr
   --,T_AP_Ref_Land.LD_Code
   --,T_AP_Gebaeude.GB_PLZ
   --,T_AP_Gebaeude.GB_Ort
   --,T_AP_Ref_Ort.ORT_Lang_EN
   --,T_AP_Ref_Region.RG_Lang_EN
   --,T_AP_Ref_Land.LD_Lang_EN 
    
   , ISNULL(RTRIM(NULLIF(T_AP_Gebaeude.GB_Strasse + ' ' + ISNULL(T_AP_Gebaeude.GB_StrasseNr, ''), '')) + ', ', '') 
   + ISNULL(T_AP_Ref_Land.LD_Code + '-' + NULLIF(T_AP_Gebaeude.GB_PLZ, '') + ', ' , '')
   + T_AP_Ref_Ort.ORT_Lang_EN 
   + ', '  
   + T_AP_Ref_Land.LD_Lang_EN
   AS GB_Adresse 
   

   ,GB_GM_Lat 
   ,GB_GM_Lng 

   ,'UPDATE T_AP_Gebaeude 
	SET  GB_GM_Lat = CAST(''' + CAST(GB_GM_Lat AS varchar(36)) + ''' AS decimal(23,20)) 
		,GB_GM_Lng = CAST(''' + CAST(GB_GM_Lng AS varchar(36))  + ''' AS decimal(23,20)) 
WHERE GB_UID = ''' + CAST(GB_UID AS varchar(36)) + '''; ' AS sql 

	-- ,'UPDATE T_AP_Gebaeude SET  GB_BK_UID = CAST(''' + CAST(GB_GK_UID AS varchar(36)) + ''' AS uniqueidentifier) WHERE GB_UID = ''' + CAST(GB_UID AS varchar(36)) + '''; ' AS GK_SQL 
FROM T_AP_Gebaeude
LEFT JOIN T_AP_Standort ON SO_UID = GB_SO_UID
LEFT JOIN T_AP_Ref_Ort ON T_AP_Ref_Ort.ORT_UID = T_AP_Standort.SO_ORT_UID
LEFT JOIN T_AP_Ref_Region ON RG_UID = ORT_RG_UID
LEFT JOIN T_AP_Ref_Land ON LD_UID = RG_LD_UID

WHERE (1=1) 

-- Alle, die Koordinaten haben 
AND NOT 
(
	(
		GB_GM_Lat IS NULL 
		OR 
		GB_GM_Lng IS NULL 
	)
	OR 
	(
		GB_GM_Lat = 0.0 
		OR 
		GB_GM_Lng = 0.0 
	)
)

-- Aber noch keine Polygone 
AND NOT EXISTS(SELECT * FROM T_ZO_Objekt_Wgs84Polygon WHERE T_ZO_Objekt_Wgs84Polygon.ZO_OBJ_WGS84_GB_UID = T_AP_Gebaeude.GB_UID ) 
