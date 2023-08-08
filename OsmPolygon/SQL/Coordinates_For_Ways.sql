
SELECT 
	 T_AP_Gebaeude.GB_UID 
	,CAST(T_AP_Gebaeude.GB_GM_Lat AS varchar(36)) 
	 + ', ' 
	 + CAST(T_AP_Gebaeude.GB_GM_Lng AS varchar(36)) 
	 AS koord 
	 
	,ISNULL(T_AP_Gebaeude.GB_Strasse, '') 
	+ ISNULL(' ' + T_AP_Gebaeude.GB_StrasseNr, '') 
	+ ', ' + ISNULL('CH-'+ T_AP_Gebaeude.GB_PLZ  + ' ', '') 
	+ ISNULL(T_AP_Gebaeude.GB_Ort, '') 
	 AS adr 

	,T_AP_Gebaeude.GB_GM_Lat 
	,T_AP_Gebaeude.GB_GM_Lng 
	 
	,'"' + CAST(T_AP_Gebaeude.GB_UID AS varchar(36)) + '",' AS gb_uids 
FROM T_AP_Gebaeude 

OUTER APPLY 
( 
	SELECT 
		COUNT(*) AS numPolygons 
	FROM T_ZO_Objekt_Wgs84Polygon 
	WHERE ZO_OBJ_WGS84_GB_UID = T_AP_Gebaeude.GB_UID 
) AS tPolygonCount 

WHERE (1=1) 
-- AND T_AP_Gebaeude.GB_GM_Lat IS NULL 
AND tPolygonCount.numPolygons = 0 
