
DECLARE @gb_uid uniqueidentifier 

;WITH CTE AS 
(
	--    SELECT '6224750E-21EB-40BA-9156-1199D6D9515C' gb_uid, '464651233' AS way	  
	      SELECT '6224750E-21EB-40BA-9156-1199D6D9515C' gb_uid, '464651232' AS way	  
	UNION SELECT 'FED82FE4-FE99-4CF4-9CCF-1CD1CA8DC18C' gb_uid, '95691336' AS way
	UNION SELECT '755B2A0E-AF02-4566-ABBE-517978ABCA33' gb_uid, '148117240' AS way
	UNION SELECT 'F41F7C00-8168-471E-9A02-7836B8AE9F4B' gb_uid, '104041936' AS way
	UNION SELECT 'F95C5A36-76AB-475A-8BC2-9E2B3818C53F' gb_uid, '43012904' AS way
	UNION SELECT 'FA546C89-0E39-433D-A6BE-A7D2B0064AE1' gb_uid, '49589463' AS way
	UNION SELECT '76E7E781-D93E-4DC1-8271-AB89FBAF8463' gb_uid, '224267897' AS way
	-- UNION SELECT '1C14B97F-D269-4DEC-9422-B1DE3A3CF762' gb_uid, '58080194' AS way
	UNION SELECT '1C14B97F-D269-4DEC-9422-B1DE3A3CF762' gb_uid, '58080208' AS way
	UNION SELECT '21C579A7-A03F-4ECF-B76D-B3516130AFF2' gb_uid, '479999588' AS way
	UNION SELECT 'C0F5CAD2-6BA8-4390-A26C-C6682D1DE4F2' gb_uid, '218557958' AS way
	UNION SELECT '0EB31953-E9E0-473A-A3F8-E9D8320E3CAF' gb_uid, '224269589' AS way
)
-- SELECT * FROM CTE WHERE gb_uid IN ('1C14B97F-D269-4DEC-9422-B1DE3A3CF762', '6224750E-21EB-40BA-9156-1199D6D9515C')

SELECT @gb_uid = (SELECT GB_UID FROM CTE WHERE way = '464651232' ) 
SELECT @gb_uid AS gb 
