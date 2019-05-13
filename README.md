# MyTrails

ASP.NET MVC Project for viewing National Park Trail Conditions along side a Google map showing selected trails in an organized fashion; it currently works with Olympic National Park only.

Conditions are scraped from official NPS websites. GeoJSON Spatial data was paired with conditions using a combination of automatic and manual systems and converted to SQL Server Spatial Data.
Some data has been left unpaired to demonstrate the manual data system.

## Why?
Current Resources either provide trail conditions in large, hard to use html tables without any indication of trail location
or require reading through extensive trail reports that often provide no relevant information. I wanted to provide a user friendly system for accessing
official conditions, posting short user conditions and posting longer trail reports and being able to mark lines of text that are specifically a trail condition
so that users can either view Trail Reports in full or just the conditions.


## Getting Started
* Clone the repository. 
* To enable full map functionality, add a file "Credentials.js" to the Credentials folder and create a variable "googlekey" equal to your API Key.
This is for local development purposes only and will expose your key if hosted.  Authorization should be run through the server in a Live Build.
Limited map functionality is still provided with the development Google Maps API.
* For creating, deleting and editing data, see Startup.cs for credentials.

### Known Issues
Cloned repositories are getting  "Could not load file or assembly 'Microsoft.WebInfrastructure' error.
Conditions interface loads and unloads conditions incorrrectly.  This will be remedied in a future JS Framework Release






