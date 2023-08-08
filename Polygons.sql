
SELECT 
	 T_AP_Gebaeude.GB_UID 
	-- ,T_AP_Gebaeude.GB_GM_Lat 
	-- ,T_AP_Gebaeude.GB_GM_Lng 
	
	,SUBSTRING(CAST(T_AP_Gebaeude.GB_GM_Lat AS varchar(30)), 1, 9)
	+ ', '
	+ SUBSTRING(CAST(T_AP_Gebaeude.GB_GM_Lng AS varchar(30)), 1, 9)
	AS coords 
	
	,T_AP_Gebaeude.GB_Nr 
	,T_AP_Ref_GebaeudeKategorie.GK_Lang_DE
	,T_AP_Gebaeude.GB_Bezeichnung 
	,T_AP_Gebaeude.GB_Strasse + ' ' + T_AP_Gebaeude.GB_StrasseNr + ', CH-' + T_AP_Gebaeude.GB_PLZ + ' ' + T_AP_Gebaeude.GB_Ort AS ADR 
	,T_AP_Gebaeude.GB_Status 
	,tPolygonCount.polyCount 
FROM T_AP_Gebaeude 
LEFT JOIN T_AP_Ref_GebaeudeKategorie ON GK_UID = GB_GK_UID 
LEFT JOIN T_AP_Standort ON SO_UID = GB_SO_UID  

OUTER APPLY 
	(
		SELECT COUNT(*) AS polyCount FROM T_ZO_Objekt_Wgs84Polygon 
		WHERE ZO_OBJ_WGS84_GB_UID = T_AP_Gebaeude.GB_UID 
	) AS tPolygonCount 

WHERE (1=1) 
AND polyCount = 0 
-- AND GB_Nr = 60 
-- AND SO_Nr = '0006'
-- AND GB_UID = '0B452FB1-2CD0-4F32-AEEE-F2ACD10B3C45' 
-- AND T_AP_Gebaeude.GB_UID IN ('F11FECEF-43CE-43EA-99A8-1F7F3477FADC', '9D4F64AD-C80B-41BF-9476-86A7B7193F67')
-- AND T_AP_Gebaeude.GB_GM_Lat IS NULL 
-- AND T_AP_Gebaeude.GB_Ort LIKE 'Winter%' 


-- https://learn.microsoft.com/en-us/visualstudio/javascript/compile-typescript-code-nuget?view=vs-2022
-- https://www.nuget.org/packages/Microsoft.TypeScript.MSBuild/5.0.1-rc



-- 93374671
-- 915BB214-0AEE-46F5-BA41-0384A4069A55

-- 119148660
-- 4CF7FFFE-FE9D-48CE-975A-2B73A02FEEDC

-- 24709795
-- FEE9A545-7150-4BBC-A364-438E7EB86C6D

-- F7648F29-C5B8-4AE2-B453-5F8BE51A33E5
-- 106880060

-- 1407142F-96BF-4024-9B9D-77731E594CB2
-- 24709790

-- C7331933-22A2-40E8-9A3B-9205568F435B
-- 24709794

-- B9A8FDB0-E6FE-48E9-A159-9AFEE3FD6A94
-- 24608736

-- A9E21C06-BBFF-4968-901D-C8CD8E3E3C03
-- 24709791

-- C31DFDF6-F8D3-4D46-9DA0-C902C9FF2C08
-- 93374758

-- 49CBDD17-BBBE-4D33-8F94-D3F7F20E19A2
-- 106880086

-- 4CA66D8F-1A59-4FEA-A975-DB8863389197
-- 660875333

-- 2B249C6B-D52F-45EB-A233-DCC8B04E9B7B
-- 24608733

-- 1409CC84-1469-4E03-9B04-E47A5D23D33E
-- 24709789

-- 913C1404-C15F-4B74-9785-FB444FBA6E48
-- 651765705




-- https://www.msn.com/de-de/lifestyle/men/wer-kann-diese-mathe-aufgabe-l%C3%B6sen-die-eine-10-j%C3%A4hrige-als-hausaufgabe-hatte/ar-AA17wxNM?ocid=msedgdhp&pc=U531&cvid=52912f43104e4734c46a57ba1f78d9e5&ei=23

-- A      = 5*B => B = A/5
--5B - 76 = B + 76
--4B - 76 = 76
--4B = 76*2
--==> B = 38
--==> A = 190

-- 190 - 76 = 114
-- 38 + 76 = 114




--H => B
--2/3 H = 1/3 H = B
--2/3 H - 12 = 1/2H
--4/6 H - 12 = 3/6 H
--4 H - 12*6 = 3H
--1H -12 *6 = 0 
--1H = 12*6 = 60 +12 = 72
--72/3 = 24
