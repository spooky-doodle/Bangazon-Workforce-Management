
-- Department Data
INSERT INTO Department ([Name], Budget) VALUES ('Marketing', 7613.63);
INSERT INTO Department ([Name], Budget) VALUES ('Sales', 31221.54);

-- Employee Data
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor) VALUES ('Maximilianus', 'Lindl', 2, 1);
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor) VALUES ('Garry', 'Levington', 2, 0);
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor) VALUES ('Roxane', 'Stirgess', 2, 0);
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor) VALUES ('Gardener', 'Mournian', 1, 0);
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSuperVisor) VALUES ('Kassandra', 'Reid', 2, 1);

-- Computer Data
INSERT INTO Computer (PurchaseDate, Make, Manufacturer) VALUES ('1/11/2017', 'GFE-744', 'Zoombeat');
INSERT INTO Computer (PurchaseDate, Make, Manufacturer) VALUES ('8/11/2015', 'YRV-483', 'Oyondu');
INSERT INTO Computer (PurchaseDate, DecommissionDate, Make, Manufacturer) VALUES ('10/30/2013', '6/22/2015', 'LBP-375', 'Eazzy');
INSERT INTO Computer (PurchaseDate, Make, Manufacturer) VALUES ('12/26/2015', 'LSU-940', 'Aibox');
INSERT INTO Computer (PurchaseDate, Make, Manufacturer) VALUES ('1/14/2019', 'MMA-189', 'Avamba');
INSERT INTO Computer (PurchaseDate, Make, Manufacturer) VALUES ('3/19/2018', 'BSC-907', 'Realbuzz');

-- ComputerEmployee Data
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) VALUES (3, 3, '4/20/2017', '2/2/2016');
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate) VALUES (1, 3, '12/26/2016');
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) VALUES (5, 6, '3/6/2015', '12/24/2015');
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate) VALUES (1, 1, '1/10/2018');
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate) VALUES (5, 6, '1/10/2016');
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate) VALUES (3, 2, '12/8/2013');
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate) VALUES (3, 3, '2/4/2016');

-- TrainingProgram Data
INSERT INTO TrainingProgram ([Name], StartDate, EndDate, MaxAttendees) VALUES ('York University', '9/12/2018', '4/29/2020', 82);
INSERT INTO TrainingProgram ([Name], StartDate, EndDate, MaxAttendees) VALUES ('Eastern Mediterranean University', '10/27/2018', '2/20/2019', 38);

-- EmployeeTraining Data
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (5, 1);
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (5, 2);
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (3, 1);
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (5, 1);

-- ProductType Data
INSERT INTO ProductType ([Name]) VALUES ('Groceries');
INSERT INTO ProductType ([Name]) VALUES ('Electronics');
INSERT INTO ProductType ([Name]) VALUES ('Apparel');

-- Customer Data
INSERT INTO Customer (FirstName, LastName, LastActiveDate, CreationDate) VALUES ('Inès', 'Northeast', '1/10/2016', '1/1/2016');
INSERT INTO Customer (FirstName, LastName, LastActiveDate, CreationDate) VALUES ('Liè', 'Dible', '1/10/2019', '10/9/2018');
INSERT INTO Customer (FirstName, LastName, LastActiveDate, CreationDate) VALUES ('Laurène', 'Hollyman', '3/23/2019', '11/1/2017');
INSERT INTO Customer (FirstName, LastName, LastActiveDate, CreationDate) VALUES ('Maëlle', 'Overill', '8/10/2019', '1/4/2018');

-- Product Data
INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, [Description], Quantity) VALUES (1, 2, '$0.61', 'Honey - Liquid', 'ligula suspendisse ornare consequat lectus in est', 25);
INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, [Description], Quantity) VALUES (1, 2, '$1.76', 'Truffle Paste', 'tempus sit amet sem fusce consequat nulla', 32);
INSERT INTO Product (ProductTypeId, CustomerId, Price, Title, [Description], Quantity) VALUES (1, 3, '$19.28', 'Sauce Bbq Smokey', 'eu mi nulla ac enim in tempor turpis nec', 17);

-- Payment Data
INSERT INTO PaymentType (AcctNumber, [Name], CustomerId) VALUES (62708, 'jcb', 2);
INSERT INTO PaymentType (AcctNumber, [Name], CustomerId) VALUES (28845, 'jcb', 3);
INSERT INTO PaymentType (AcctNumber, [Name], CustomerId) VALUES (16797, 'diners-club-enroute', 4);
INSERT INTO PaymentType (AcctNumber, [Name], CustomerId) VALUES (622854, 'mastercard', 3);

-- Order Data
INSERT INTO [Order] (CustomerId, PaymentTypeId) VALUES (2, 1);
INSERT INTO [Order] (CustomerId, PaymentTypeId) VALUES (3, 2);
INSERT INTO [Order] (CustomerId) VALUES (4);

-- OrderProduct Data
INSERT INTO OrderProduct (OrderId, ProductId) VALUES (3, 2);
INSERT INTO OrderProduct (OrderId, ProductId) VALUES (1, 1);
INSERT INTO OrderProduct (OrderId, ProductId) VALUES (2, 1);
INSERT INTO OrderProduct (OrderId, ProductId) VALUES (3, 2);
INSERT INTO OrderProduct (OrderId, ProductId) VALUES (3, 1);