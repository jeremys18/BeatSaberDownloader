/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

:r .\Scripts\Tables\Characteristic.sql
:r .\Scripts\Tables\DeclaredAI.sql
:r .\Scripts\Tables\Difficulty.sql
:r .\Scripts\Tables\Environment.sql
:r .\Scripts\Tables\Sentiment.sql
:r .\Scripts\Tables\State.sql
:r .\Scripts\Tables\UserType.sql
