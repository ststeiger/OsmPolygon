-- https://maps.googleapis.com/maps/api/geocode/json?address=Bern Helvetiastrasse 37

SELECT 
	 T_GebaeudeIMMO.GBI_SO_UID AS SO_UID 
	,T_GebaeudeIMMO.GBI_UID AS GB_UID 
	,T_GebaeudeIMMO.GBI_Nr AS GB_Nr 
	,T_GebaeudeIMMO.GBI_Name AS GB_Bezeichnung
	,T_GebaeudeIMMO.GBI_Kennzeichen 
	 
   --,T_AP_Gebaeude.GB_Strasse
   --,T_AP_Gebaeude.GB_StrasseNr
   --,T_AP_Ref_Land.LD_Code
   --,T_AP_Gebaeude.GB_PLZ
   --,T_AP_Gebaeude.GB_Ort
   --,T_AP_Ref_Ort.ORT_Lang_EN
   --,T_AP_Ref_Region.RG_Lang_EN
   --,T_AP_Ref_Land.LD_Lang_EN 
     
	, COALESCE(RTRIM(NULLIF(T_GebaeudeIMMO.GBI_StrasseCAFM + ' ' + COALESCE(T_GebaeudeIMMO.GBI_HausnrCAFM, ''), '')) + ', ', '') 
	+ COALESCE(T_Land.LD_KZ + '-' + NULLIF(T_GebaeudeIMMO.GBI_PlzCAFM, '') + ', ' , '') 
	+ T_GebaeudeIMMO.GBI_OrtCAFM 
	+ ', ' 
	+ T_Land.LD_Name 
	 AS GB_Adresse 
     
	,T_GebaeudeIMMO.GBI_GM_Lat AS GB_GM_Lat
	,T_GebaeudeIMMO.GBI_GM_Lng AS GB_GM_Lng
	 
	,'UPDATE T_AP_Gebaeude 
	SET  GBI_GM_Lat = CAST(''' + CAST(T_GebaeudeIMMO.GBI_GM_Lat AS character varying(36)) + ''' AS decimal(23, 20) ) 
		,GBI_GM_Lng = CAST(''' + CAST(T_GebaeudeIMMO.GBI_GM_Lng AS character varying(36)) + ''' AS decimal(23, 20) ) 
WHERE GBI_UID = ''' + CAST(T_GebaeudeIMMO.GBI_UID AS character varying(36)) + '''; ' AS sql 
	
	-- ,'UPDATE T_AP_Gebaeude SET  GB_BK_UID = CAST(''' + CAST(GB_GK_UID AS character varying(36)) + ''' AS uniqueidentifier) WHERE GB_UID = ''' + CAST(GB_UID AS character varying(36)) + '''; ' AS GK_SQL 
FROM T_GebaeudeIMMO

LEFT JOIN T_Standort 
	ON T_Standort.SO_UID = T_GebaeudeIMMO.GBI_SO_UID 

LEFT JOIN T_Vermessungsbezirk 
	ON T_Vermessungsbezirk.VB_ApertureID = T_Standort.SO_VB_UID 
	AND T_Vermessungsbezirk.VB_Status = 1 
	AND (T_Vermessungsbezirk.VB_DatumVon <= CURRENT_TIMESTAMP) 
	AND (T_Vermessungsbezirk.VB_DatumBis >= CURRENT_TIMESTAMP) 

LEFT JOIN T_Kreis 
	ON T_Kreis.KS_ApertureID = T_Vermessungsbezirk.VB_KS_UID 
	AND T_Kreis.KS_Status = 1 
	AND (T_Kreis.KS_DatumVon <= CURRENT_TIMESTAMP) 
	AND (T_Kreis.KS_DatumBis >= CURRENT_TIMESTAMP) 
	
LEFT JOIN T_Gemeinde 
	ON T_Gemeinde.GM_ApertureID = T_Kreis.KS_GM_UID 
	AND T_Gemeinde.GM_Status = 1
	AND (T_Gemeinde.GM_DatumBis >= CURRENT_TIMESTAMP) 
	AND (T_Gemeinde.GM_DatumVon <= CURRENT_TIMESTAMP)

LEFT JOIN T_Ref_Gemeindeteilung 
	ON T_Ref_Gemeindeteilung.GT_UID = T_Gemeinde.GM_GT_UID 
	AND T_Ref_Gemeindeteilung.GT_Status = 1

LEFT JOIN T_Region 
	ON T_Region.RG_UID = T_Gemeinde.GM_RG_UID
	-- AND T_Region.RG_Status = 1 
	-- AND (T_Region.RG_DatumVon >= CURRENT_TIMESTAMP) 
	-- AND (T_Region.RG_DatumBis <= CURRENT_TIMESTAMP)

LEFT JOIN T_Land 
	ON T_Land.LD_UID = T_Region.RG_LD_UID 
	-- AND T_Land.LD_Status = 1 
	-- AND (T_Land.LD_DatumVon >= CURRENT_TIMESTAMP) 
	-- AND (T_Land.LD_DatumBis <= CURRENT_TIMESTAMP)

LEFT JOIN T_Ref_Landesteile 
	ON T_Ref_Landesteile.LT_UID = T_Land.LD_LT_UID 
	AND T_Ref_Landesteile.LT_Status = 1 

WHERE (1=1) 

-- Alle, die Koordinaten haben 
AND NOT 
(
	(
		GBI_GM_Lat IS NULL 
		OR 
		GBI_GM_Lng IS NULL 
	)
	OR 
	(
		GBI_GM_Lat = 0.0 
		OR 
		GBI_GM_Lng = 0.0 
	)
)

-- Aber noch keine Polygone 
AND NOT EXISTS(SELECT * FROM T_ZO_Objekt_Wgs84Polygon WHERE T_ZO_Objekt_Wgs84Polygon.ZO_OBJ_WGS84_GB_UID = T_GebaeudeIMMO.GBI_UID ) 
