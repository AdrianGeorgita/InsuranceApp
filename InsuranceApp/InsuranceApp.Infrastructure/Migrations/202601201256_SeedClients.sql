INSERT INTO dbo.Clients(
	[Id],
	[Type],
	[Name],
	[IdentificationNumber],
	[Email],
	[Phone],
	[Address]
)
VALUES
	-- Individuals
	('7fc17474-00f8-424a-9a0c-58fbddd1db73', 'Individual', 'John Doe', '3021212123123', 'john.doe@mail.com', '0712345678', 'Some random street No.1'),
	('1b2d7b9f-3c9e-4c6f-9f63-1f6f5f9c2a01', 'Individual', 'Alice Popescu', '2960512123456', 'alice.popescu@mail.com', '0723456789', 'Str. Lalelelor Nr. 10, Bucharest'),
	('6a8f1f0d-9a8c-4c42-8bfa-3d4a5b7e9c22', 'Individual', 'Mihai Ionescu', '1901022333444', 'mihai.ionescu@mail.com', '0734567890', 'Bd. Independentei Nr. 45, Cluj-Napoca'),
	('a1d4f1a7-1f2e-4b5b-9a2d-01f1e8a3c001', 'Individual', 'Andrei Marinescu', '1950305123456', 'andrei.marinescu@mail.com', '0721112233', 'Str. Mihai Eminescu Nr. 12, Bucharest'),
	('b2c5a6e3-4d91-4e1b-8b3a-12c9f3d4a002', 'Individual', 'Elena Dumitrescu', '2960829123456', 'elena.dumitrescu@mail.com', '0732223344', 'Bd. Unirii Nr. 88, Bucharest'),
	('c3e6b7d4-5a82-42c9-9e5b-23d8a4b5c003', 'Individual', 'Radu Stan', '1891217123456', 'radu.stan@mail.com', '0743334455', 'Str. Avram Iancu Nr. 6, Cluj-Napoca'),
	('d4f7c8e5-6b73-4a8d-8f6c-34e9b5c6d004', 'Individual', 'Ioana Pavel', '2970401123456', 'ioana.pavel@mail.com', '0754445566', 'Str. Libertatii Nr. 14, Timisoara'),
	('e5a8d9f6-7c64-4b7e-9a7d-45f1c6d7e005', 'Individual', 'Cristian Pop', '1910618123456', 'cristian.pop@mail.com', '0765556677', 'Str. Horea Nr. 21, Oradea'),
	('f6b9e0a7-8d55-4c6f-8b8e-56a2d7e8f006', 'Individual', 'Simona Rusu', '2920922123456', 'simona.rusu@mail.com', '0776667788', 'Str. Stefan cel Mare Nr. 33, Iasi'),
	('07c0f1b8-9e46-4d5a-9c9f-67b3e8f9a007', 'Individual', 'Bogdan Neagu', '1881105123456', 'bogdan.neagu@mail.com', '0787778899', 'Str. Traian Nr. 9, Braila'),
	('18d1a2c9-0f37-4e4b-8d0a-78c4f9a0b008', 'Individual', 'Laura Enache', '2980202123456', 'laura.enache@mail.com', '0798889900', 'Str. Republicii Nr. 55, Pitesti'),
	-- Companies
	('9d3e6c21-1a7b-4f5e-9d1c-4e8a9b0c1234', 'Company', 'Tech Solutions SRL', '12345678', 'contact@techsolutions.ro', '0312345678', 'Str. Industriilor Nr. 5, Bucharest'),
	('4f8b2c91-7e3a-4b6a-8c4f-2d9a1e5b6789', 'Company', 'Green Energy SA', '87654321', 'office@greenenergy.ro', '0264123456', 'Calea Dorobantilor Nr. 120, Cluj-Napoca'),
	('e7a1c9b4-5f6d-4b2a-9c8e-0a1b2c3d4e56', 'Company', 'LogiTrans Group SRL', '41239876', 'info@logitrans.ro', '0213456789', 'Sos. Bucuresti-Ploiesti Nr. 15, Otopeni'),
	('29e2b3d0-1a28-4f3c-9e1b-89d5a0b1c009', 'Company', 'Alpha Construct SRL', '23456789', 'office@alphaconstruct.ro', '0214567890', 'Str. Constructorilor Nr. 18, Bucharest'),
	('3af3c4e1-2b19-4a2d-8f2c-90e6b1c2d010', 'Company', 'BlueWave IT SRL', '34567891', 'contact@bluewaveit.ro', '0315678901', 'Bd. Pipera Nr. 101, Bucharest'),
	('4b04d5f2-3c0a-4b1e-9a3d-a1f7c2d3e011', 'Company', 'Nord Logistics SA', '45678912', 'info@nordlogistics.ro', '0264456789', 'Str. Portului Nr. 7, Constanta'),
	('5c15e603-4d1b-4c0f-8b4e-b2a8d3e4f012', 'Company', 'EcoFarm Distribution SRL', '56789123', 'sales@ecofarm.ro', '0232456789', 'Str. Agriculturii Nr. 25, Bacau'),
	('6d26f714-5e2c-4d9a-9c5f-c3b9e4f5a013', 'Company', 'Urban Design Studio SRL', '67891234', 'office@urbandesign.ro', '0216789123', 'Str. Arhitectilor Nr. 4, Bucharest'),
	('7e370825-6f3d-4e8b-8d60-d4c0f5a6b014', 'Company', 'Rapid Courier Express SRL', '78912345', 'support@rapidcourier.ro', '0317891234', 'Sos. Giurgiului Nr. 210, Bucharest'),
	('8f481936-704e-4f7c-9e71-e5d1a6b7c015', 'Company', 'Medical Plus Clinic SRL', '89123456', 'receptie@medicalplus.ro', '0218912345', 'Str. Sanatatii Nr. 3, Bucharest'),
	('90592a47-815f-4a6d-8f82-f6e2b7c8d016', 'Company', 'AutoPro Service SRL', '91234567', 'contact@autoproservice.ro', '0256912345', 'Str. Motorului Nr. 19, Pitesti'),
	('a16a3b58-9260-4b5e-9a93-07f3c8d9e017', 'Company', 'Smart Retail Group SA', '12349876', 'office@smartretail.ro', '0212349876', 'Bd. Comerciala Nr. 56, Bucharest'),
	('b27b4c69-a371-4c4f-8b04-18a4d9e0f018', 'Company', 'Aqua Systems SRL', '23459876', 'info@aquasystems.ro', '0241234987', 'Str. Izvoarelor Nr. 11, Ploiesti'),
	('c38c5d7a-b482-4d3a-9c15-29b5e0f1a019', 'Company', 'Vision Advertising SRL', '34569876', 'contact@visionadv.ro', '0213456987', 'Str. Publicitatii Nr. 8, Bucharest'),
	('d49d6e8b-c593-4e2b-8d26-3ac6f1a2b020', 'Company', 'Industrial Parts Supply SRL', '45679876', 'sales@industrialparts.ro', '0268456798', 'Str. Fabricii Nr. 30, Brasov');