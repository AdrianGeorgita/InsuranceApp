INSERT INTO dbo.Currencies(
	[Code],
	[Name],
	[ExchangeRateToBase],
	[IsActive]
)
VALUES
	('RON', 'Romanian Leu',        1.0000, 1),
	('EUR', 'Euro',               4.9700, 1),
	('USD', 'US Dollar',          4.5800, 1),
	('GBP', 'British Pound',      5.8200, 1),
	('CHF', 'Swiss Franc',        5.2500, 1),
	('HUF', 'Hungarian Forint',   0.0127, 1),
	('PLN', 'Polish Zloty',        1.1500, 1),
	('CZK', 'Czech Koruna',        0.2000, 1),
	('BGN', 'Bulgarian Lev',       2.5400, 1),
	('SEK', 'Swedish Krona',       0.4400, 1),
	('NOK', 'Norwegian Krone',     0.4300, 1),
	('DKK', 'Danish Krone',        0.6700, 1),
	('JPY', 'Japanese Yen',        0.0310, 1),
	('CAD', 'Canadian Dollar',    3.3800, 1),
	('AUD', 'Australian Dollar',  3.0200, 1),
	('CNY', 'Chinese Yuan',        0.6400, 1),
	('TRY', 'Turkish Lira',        0.1400, 1),
	('RUB', 'Russian Ruble',       0.0500, 0),
	('INR', 'Indian Rupee',        0.0550, 0),
	('BRL', 'Brazilian Real',      0.9200, 0);


INSERT INTO dbo.FeeConfigurations(
	[Id],
	[Name],
	[Type],
	[Percentage],
	[EffectiveFrom],
	[EffectiveTo],
	[IsActive]
)
VALUES
	('A6F0B8A1-4F3E-4B7D-9C6E-1D9E0B6C2A01', 'Standard broker fee',          'BrokerCommission', 0.025, '2024-01-01', '2026-12-31', 1),
	('B1D2C3E4-1A2B-4C3D-8E9F-2A3B4C5D6E02', 'Earthquake risk zone adjustment',   'RiskAdjustment',   0.08, '2023-06-01', '2026-06-30', 1),
	('C9E8D7C6-5B4A-4D3C-9B8A-3C2B1A0F9E03', 'Flood zone adjustment',        'RiskAdjustment',   0.075, '2022-01-01', '2024-12-31', 0),
	('D0A1B2C3-6D5E-4F3A-8B7C-4D5E6F7A8B04', 'Wildfire risk adjustment',     'RiskAdjustment',   0.1, '2021-05-15', '2023-12-31', 1);


INSERT INTO dbo.RiskFactorConfigurations(
	[Id],
	[Level],
	[ReferenceId],
	[BuildingType],
	[AdjustmentPercentage],
	[IsActive]
)
VALUES
	('a1c2b3d4-e111-4a9f-9b01-111111111111', 'Country', '5346db5a-3b6c-4c36-ad06-de647f3c873a', NULL,   0.2, 1),
	('b2d3c4a5-e222-4b8e-9b02-222222222222', 'Country', '927e1b08-0e76-4966-9748-b48c89ee946d', NULL,  0.3, 1),
	('f2a3b4c5-eccc-4544-9b12-cccccccccccc', 'Country', 'fc129fe5-2737-46ce-9b16-6fa3e01bb046', NULL,  0.09, 1),

	('c3d4e5f6-e333-4c7d-9b03-333333333333', 'County',  '8b9d2b2e-7f6d-4c2f-8b29-3c8c9d2a4d14', NULL,     0.08, 1),
	('d4e5f6a7-e444-4d6c-9b04-444444444444', 'County',  '0c0a48a5-7d6a-4f34-86f2-1f7f7bbbf6b1', NULL,    -0.3, 1),
	('e5f6a7b8-e555-4e5b-9b05-555555555555', 'County',  '3c4d5e6f-7071-49aa-cd0e-606162636465', NULL,     0.15, 0),

	('f6a7b8c9-e666-4f4a-9b06-666666666666', 'City',    'a0010001-1111-4111-8111-000000000001', NULL,     0.06, 1),
	('a7b8c9d0-e777-4049-9b07-777777777777', 'City',    'a0030001-3333-4333-8333-000000000001', NULL,    -0.20, 1),
	('b8c9d0e1-e888-4148-9b08-888888888888', 'City',    'a0040001-4444-4444-8444-000000000001', NULL,     0.11, 0),

	('c9d0e1f2-e999-4247-9b09-999999999999', 'BuildingType', NULL, 'Residential',  -0.1, 1),
	('d0e1f2a3-eaaa-4346-9b10-aaaaaaaaaaaa', 'BuildingType',  NULL, 'Industrial',  0.1, 1),
	('e1f2a3b4-ebbb-4445-9b11-bbbbbbbbbbbb', 'BuildingType',      NULL, 'Office',  -0.2, 1);


INSERT INTO dbo.Administrators(
	[Id],
	[Name],
	[Email],
	[Role]
)
VALUES
	('a1111111-1111-4111-8111-111111111111', 'Andrei Popescu',   'andrei.popescu@insuranceapp.com', 'Admin'),
	('b2222222-2222-4222-8222-222222222222', 'Maria Ionescu',    'maria.ionescu@insuranceapp.com',  'Manager'),
	('c3333333-3333-4333-8333-333333333333', 'John Smith',       'john.smith@insuranceapp.com',     'Admin'),
	('d4444444-4444-4444-8444-444444444444', 'Elena Dumitrescu', 'elena.dumitrescu@insuranceapp.com','Manager');


INSERT INTO dbo.Brokers(
	[Id],
	[Code],
	[Name],
	[Email],
	[Phone],
	[Status],
	[CommissionPercentage]
)
VALUES
	('a0f1b2c3-1111-4111-8111-111111111111', 'BRK-RO-001', 'Alpha Insurance Brokers',     'contact@alpha-brokers.ro',     '+40-721-000-001', 1,   1.20),
	('b1a2c3d4-2222-4222-8222-222222222222', 'BRK-RO-002', 'Blue Shield Brokers',         'office@blueshield.ro',         '+40-721-000-002', 1,   1.00),
	('c2b3d4e5-3333-4333-8333-333333333333', 'BRK-RO-003', 'Carpathia Insurance',          'hello@carpathia.ro',           '+40-721-000-003', 1,   1.50),
	('d3c4e5f6-4444-4444-8444-444444444444', 'BRK-RO-004', 'Danube Risk Advisors',         'info@danube-risk.ro',          '+40-721-000-004', 1,   0.90),
	('e4d5f6a7-5555-4555-8555-555555555555', 'BRK-RO-005', 'Eastern Coverage Group',        'contact@easterncoverage.ro',   '+40-721-000-005', 1,   1.10),
	('f5e6a7b8-6666-4666-8666-666666666666', 'BRK-RO-006', 'Frontline Brokers',             'sales@frontline.ro',           '+40-721-000-006', 1,   0.80),
	('a6f7b8c9-7777-4777-8777-777777777777', 'BRK-RO-007', 'Guardian Insurance Solutions',  'office@guardian.ro',           '+40-721-000-007', 1,   1.30),
	('b7a8c9d0-8888-4888-8888-888888888888', 'BRK-RO-008', 'Horizon Brokers',               'contact@horizon.ro',           '+40-721-000-008', 1,   1.00),
	('c8b9d0e1-9999-4999-8999-999999999999', 'BRK-RO-009', 'Ironclad Risk Partners',        'info@ironclad.ro',             '+40-721-000-009', 1,   0.70),
	('d9c0e1f2-aaaa-4aaa-8aaa-aaaaaaaaaaaa', 'BRK-RO-010', 'Jupiter Insurance Brokers',     'hello@jupiter.ro',             '+40-721-000-010', 1,   NULL),
	('e0d1f2a3-bbbb-4bbb-8bbb-bbbbbbbbbbbb', 'BRK-RO-011', 'Keystone Advisory',              'office@keystone.ro',           '+40-721-000-011', 1,   0.95),
	('f1e2a3b4-cccc-4ccc-8ccc-cccccccccccc', 'BRK-RO-012', 'Lionheart Brokers',              'contact@lionheart.ro',         '+40-721-000-012', 1,   1.05),
	('a2f3b4c5-dddd-4ddd-8ddd-dddddddddddd', 'BRK-RO-013', 'Mercury Insurance Consulting',  'info@mercury.ro',              '+40-721-000-013', 1, 1.025),
	('b3a4c5d6-eeee-4eee-8eee-eeeeeeeeeeee', 'BRK-RO-014', 'NorthStar Brokers',              'office@northstar.ro',          '+40-721-000-014', 0, NULL);


INSERT INTO dbo.Policies(
	[PolicyNumber],
	[ClientId],
	[BuildingId],
	[BrokerId],
	[Status],
	[StartDate],
	[EndDate],
	[BasePremium],
	[CurrencyCode],
	[FinalPremium]
)
VALUES
	('POL-00001', '7fc17474-00f8-424a-9a0c-58fbddd1db73', 'a1000001-0000-4000-8000-000000000001', 'a0f1b2c3-1111-4111-8111-111111111111', 'Expired',   '2020-02-01', '2021-02-01',  4250.00, 'RON',  4352.61),
	('POL-00002', '1b2d7b9f-3c9e-4c6f-9f63-1f6f5f9c2a01', 'a1000002-0000-4000-8000-000000000002', 'b1a2c3d4-2222-4222-8222-222222222222', 'Active',    '2022-06-15', '2027-06-14',  2150.00, 'EUR',  2197.56),
	('POL-00003', '6a8f1f0d-9a8c-4c42-8bfa-3d4a5b7e9c22', 'a1000003-0000-4000-8000-000000000003', 'c2b3d4e5-3333-4333-8333-333333333333', 'Active',    '2023-01-01', '2026-12-31',  9800.00, 'USD', 10066.36),
	('POL-00004', 'a1d4f1a7-1f2e-4b5b-9a2d-01f1e8a3c001', 'a1000004-0000-4000-8000-000000000004', 'd3c4e5f6-4444-4444-8444-444444444444', 'Expired',   '2019-09-01', '2020-09-01',  3625.50, 'GBP',  3702.03),
	('POL-00005', 'b2c5a6e3-4d91-4e1b-8b3a-12c9f3d4a002', 'a1000005-0000-4000-8000-000000000005', 'e4d5f6a7-5555-4555-8555-555555555555', 'Active',    '2021-04-10', '2026-04-09',  1290.00, 'RON',  1319.84),
	('POL-00006', 'c3e6b7d4-5a82-42c9-9e5b-23d8a4b5c003', 'a1000006-0000-4000-8000-000000000006', 'f5e6a7b8-6666-4666-8666-666666666666', 'Draft',     '2024-03-01', '2027-02-28',   895.00, 'EUR',   912.99),
	('POL-00007', 'd4f7c8e5-6b73-4a8d-8f6c-34e9b5c6d004', 'a1000007-0000-4000-8000-000000000007', 'a6f7b8c9-7777-4777-8777-777777777777', 'Expired',   '2020-07-20', '2021-07-19',   420.00, 'RON',   430.57),
	('POL-00008', 'e5a8d9f6-7c64-4b7e-9a7d-45f1c6d7e005', 'a1000008-0000-4000-8000-000000000008', 'b7a8c9d0-8888-4888-8888-888888888888', 'Active',    '2022-11-01', '2026-11-01', 11850.00, 'USD', 12112.12),
	('POL-00009', 'f6b9e0a7-8d55-4c6f-8b8e-56a2d7e8f006', 'a1000009-0000-4000-8000-000000000009', 'c8b9d0e1-9999-4999-8999-999999999999', 'Active',    '2025-01-01', '2026-12-31',  2450.00, 'GBP',  2496.76),
	('POL-00010', '07c0f1b8-9e46-4d5a-9c9f-67b3e8f9a007', 'a1000010-0000-4000-8000-000000000010', 'd9c0e1f2-aaaa-4aaa-8aaa-aaaaaaaaaaaa', 'Expired',   '2018-05-10', '2019-05-09',  7600.00, 'RON',  7798.88),
	('POL-00011', '18d1a2c9-0f37-4e4b-8d0a-78c4f9a0b008', 'a1000011-0000-4000-8000-000000000011', 'e0d1f2a3-bbbb-4bbb-8bbb-bbbbbbbbbbbb', 'Draft',     '2023-08-01', '2027-07-31', 15200.00, 'EUR', 15528.53),
	('POL-00012', '9d3e6c21-1a7b-4f5e-9d1c-4e8a9b0c1234', 'a1000012-0000-4000-8000-000000000012', 'f1e2a3b4-cccc-4ccc-8ccc-cccccccccccc', 'Active',    '2021-12-01', '2026-11-30', 13950.00, 'RON', 14265.63),
	('POL-00013', '4f8b2c91-7e3a-4b6a-8c4f-2d9a1e5b6789', 'a1000013-0000-4000-8000-000000000013', 'a0f1b2c3-1111-4111-8111-111111111111', 'Active',    '2020-10-15', '2026-10-14', 17400.00, 'USD', 17820.11),
	('POL-00014', 'e7a1c9b4-5f6d-4b2a-9c8e-0a1b2c3d4e56', 'a1000014-0000-4000-8000-000000000014', 'b1a2c3d4-2222-4222-8222-222222222222', 'Active',    '2024-09-01', '2027-08-31',  3990.00, 'CHF',  4078.26),
	('POL-00015', '29e2b3d0-1a28-4f3c-9e1b-89d5a0b1c009', 'a1000015-0000-4000-8000-000000000015', 'c2b3d4e5-3333-4333-8333-333333333333', 'Expired',   '2019-03-01', '2020-03-01',  8650.00, 'BGN',  8885.11),
	('POL-00016', '3af3c4e1-2b19-4a2d-8f2c-90e6b1c2d010', 'a1000016-0000-4000-8000-000000000016', 'd3c4e5f6-4444-4444-8444-444444444444', 'Active',    '2022-02-01', '2026-02-01',  7180.00, 'EUR',  7331.56),
	('POL-00017', '4b04d5f2-3c0a-4b1e-9a3d-a1f7c2d3e011', 'a1000017-0000-4000-8000-000000000017', 'e4d5f6a7-5555-4555-8555-555555555555', 'Active',    '2025-06-01', '2028-05-31',  6420.00, 'USD',  6568.51),
	('POL-00018', '5c15e603-4d1b-4c0f-8b4e-b2a8d3e4f012', 'a1000018-0000-4000-8000-000000000018', 'f5e6a7b8-6666-4666-8666-666666666666', 'Active',    '2020-11-20', '2026-11-19',  9800.00, 'GBP',  9996.94),
	('POL-00019', '6d26f714-5e2c-4d9a-9c5f-c3b9e4f5a013', 'a1000019-0000-4000-8000-000000000019', 'a6f7b8c9-7777-4777-8777-777777777777', 'Expired',   '2017-01-01', '2018-01-01',  4050.00, 'RON',  4151.88),
	('POL-00020', '7e370825-6f3d-4e8b-8d60-d4c0f5a6b014', 'a1000020-0000-4000-8000-000000000020', 'b7a8c9d0-8888-4888-8888-888888888888', 'Draft',     '2023-05-01', '2026-05-01',  5200.00, 'EUR',  5315.02),
	('POL-00021', '8f481936-704e-4f7c-9e71-e5d1a6b7c015', 'a1000021-0000-4000-8000-000000000021', 'c8b9d0e1-9999-4999-8999-999999999999', 'Active',    '2024-01-01', '2027-12-31',  4900.00, 'HUF',  4993.51),
	('POL-00022', '90592a47-815f-4a6d-8f82-f6e2b7c8d016', 'a1000022-0000-4000-8000-000000000022', 'd9c0e1f2-aaaa-4aaa-8aaa-aaaaaaaaaaaa', 'Cancelled', '2021-07-01', '2026-06-30', 13450.00, 'USD', 13801.96),
	('POL-00023', 'a16a3b58-9260-4b5e-9a93-07f3c8d9e017', 'a1000023-0000-4000-8000-000000000023', 'e0d1f2a3-bbbb-4bbb-8bbb-bbbbbbbbbbbb', 'Expired',   '2016-09-15', '2017-09-14',  3720.00, 'RON',  3800.40),
	('POL-00024', 'b27b4c69-a371-4c4f-8b04-18a4d9e0f018', 'a1000024-0000-4000-8000-000000000024', 'f1e2a3b4-cccc-4ccc-8ccc-cccccccccccc', 'Active',    '2024-05-01', '2026-11-01',  6150.00, 'EUR',  6289.15),
	('POL-00025', 'c38c5d7a-b482-4d3a-9c15-29b5e0f1a019', 'a1000025-0000-4000-8000-000000000025', 'a0f1b2c3-1111-4111-8111-111111111111', 'Active',    '2022-10-01', '2026-09-30',  2990.00, 'RON',  3062.19);