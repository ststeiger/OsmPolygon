
SELECT 
	GB_UID, 
	GB_Strasse 
	+ ' ' 
	+ GB_StrasseNr
	+ ', CH-' 
	+ GB_PLZ
	+ ' ' 
	+ GB_Ort
	AS bez 

	,GB_GM_Lat
	,GB_GM_Lng
	,GB_GM_SVLat
	,GB_GM_SVLng

FROM T_AP_Gebaeude 
-- WHERE GB_Strasse LIKE 'Alexandraweg'
