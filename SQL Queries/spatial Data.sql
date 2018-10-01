SELECT Trails.TrailName,  
geography::UnionAggregate(Trailsections.Geography)
FROM Trails, TrailSections 
GROUP BY Trails.TrailName