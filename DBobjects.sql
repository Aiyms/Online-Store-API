--БД:
CREATE database storedb;
USE storedb;

--Таблицы:
CREATE TABLE Goods(
	Id NVARCHAR(max) NOT NULL,
	Title NVARCHAR(max),
	Price Decimal(18,2)
)
INSERT INTO Goods(Id, Title, Price)
VALUES (NEWID(), 'Смартфон Apple IPhone 5C, 16 GB, White', 569990),
	   (NEWID(), 'Смартфон Apple IPhone 5C, 64 GB, White', 501990),
	   (NEWID(), 'Планшет Apple new iPad, 16GB, 4G, Black (MD366RS/A)', 407990),
	   (NEWID(), 'Apple Wireless Keyboard  Model: A1314 (MC184RS/B)', 69990),
	   (NEWID(), 'Планшет Apple new iPad, 32GB, 4G, Black (MD367RS/A)', 507990),
	   (NEWID(), 'Планшет Apple new iPad, 64GB, 4G, White (MD371RS/A)', 579990),
	   (NEWID(), 'Мышь Apple Magic Mouse  Model: A1296 (NZMB829ZMA)', 39990),
	   (NEWID(), 'Apple Magic Trackpad. Model: A1339 (MC380ZM/A)', 141099),
	   (NEWID(), 'Ноутбук Apple MacBook Air 13''. Model: A1369 Core 2 Duo 1.86GHz/2GB/256GB flash/GeForce 320M  Russian (MC504RS/A)', 1400000),
	   (NEWID(), 'Чехол Apple iPad 2 Smart Cover Pink (MC941ZM/A)', 9990)

CREATE TABLE Basket(
	Id NVARCHAR(max) NOT NULL,
	Amount Decimal(18,2)
)

CREATE TABLE Quantity(
	GoodId NVARCHAR(max),
	Quantity INT,
	BasketId NVARCHAR(max)
)

CREATE TABLE Orders(
	Id NVARCHAR(max) NOT NULL,
	BasketId NVARCHAR(max),
	CreatedDate datetime
)

CREATE TABLE OrdersDetails(
	Id NVARCHAR(max) NOT NULL,
	OrderId NVARCHAR(max),
	CardNumber NVARCHAR(max),
	Address NVARCHAR(max)
)
--Cправочник
CREATE TABLE OrderStatus(
	Id NVARCHAR(max) NOT NULL,
	Code INT,
	Description NVARCHAR(max)
)
INSERT INTO OrderStatus(Id, Code, Description)
VALUES 
	   (NEWID(), 1,'Created'),
	   (NEWID(), 2,'PAID'),
	   (NEWID(), 3,'FINILIZED')

CREATE TABLE OrderStatusHistory(
	Id NVARCHAR(max) NOT NULL,
	CreatedDate datetime,
	OrderId NVARCHAR(max),
	OrderStatusId NVARCHAR(max) 
)

--Процедуры
----------------- 1 -----------------------------
CREATE PROCEDURE [dbo].[p_AddProductToBasket]
	@GoodId NVARCHAR(max)
AS
BEGIN
	DECLARE @Price DECIMAL;
	SET @Price = (select Price from Goods s where s.Id = @GoodId)

	DECLARE @BasketIdCount NVARCHAR(max); -- так как у нас один пользователь - значит одна корзина
	SET @BasketIdCount = (select count(s.BasketId) from Quantity s /*where s.GoodId = @GoodId*/)

	DECLARE @NewBasketId NVARCHAR(max);
	SET @NewBasketId = NEWID();

	IF(@BasketIdCount = 0)
		begin
			INSERT INTO Basket (Id, Amount) VALUES (@NewBasketId , @Price)
			INSERT INTO Quantity (GoodId, Quantity, BasketId) VALUES (@GoodId, 1, @NewBasketId)
			select NewBasketId = @NewBasketId
		end
	else if (@BasketIdCount > 0) 
		begin
			declare @CurrentBasketId NVARCHAR(max) = (select Id from Basket)
			update Basket set Amount = Amount + @Price where  Id = @CurrentBasketId
			INSERT INTO Quantity (GoodId, Quantity, BasketId) VALUES (@GoodId, 1, @CurrentBasketId)
			--select NewBasketId = @CurrentBasketId 
		end 
END

----------------- 2 -----------------------------
CREATE PROCEDURE [dbo].[p_ChangeProductQuantityInBasket]
	@GoodId NVARCHAR(max),
	@Quantity INT,
	@BasketId NVARCHAR(max)

AS
BEGIN
	
	declare @CurrentQuantity INT = (select q.Quantity from Quantity q where q.BasketId = @BasketId and q.GoodId = @GoodId)
	declare @Price decimal = (select g.Price from Goods g where g.Id = @GoodId)

	-- Если число отрицательное и по модулю равен кол-ву товара в корзине, то удаляем товар и изменяем сумму в корзине
	if(@Quantity <= 0 and abs(@Quantity) = @CurrentQuantity)
		begin
			delete from Quantity
			--update Quantity set isDeleted = 1 
			where BasketId = @BasketId and GoodId = @GoodId
			
			update Basket set Amount = Amount - (abs(@Quantity)*@Price)
			where Id = @BasketId
		end
	-- Если число отрицательное и по модулю меньше кол-ва товаров в корзине, то изменяем кол-во и сумму
	else if(@Quantity <= 0 and abs(@Quantity) < @CurrentQuantity)
		begin
			update Quantity set Quantity = @CurrentQuantity-abs(@Quantity) 
			where BasketId = @BasketId and GoodId = @GoodId

			update Basket set Amount = Amount - (abs(@Quantity)*@Price) 
			where Id = @BasketId
		end
	-- Если число положительное значит увеличиваем кол-во товара на это число и изменяем сумму в корзине
	else if(@Quantity > 0)
		begin 
			update Quantity set Quantity = Quantity + @Quantity
			where BasketId = @BasketId and GoodId = @GoodId

			update Basket set Amount = Amount + (@Quantity *@Price) 
			where Id = @BasketId
		end


END
----------------- 3 -----------------------------
CREATE PROCEDURE [dbo].[p_CreateOrder]
	@BasketId NVARCHAR(max)
	--@Address NVARCHAR(max),
	--@CardNumber NVARCHAR(max)
AS
BEGIN
	declare @OrderId NVARCHAR(max) = NEWID()

	insert into Orders(Id, BasketId, CreatedDate)
	values(@OrderId, @BasketId, SYSDATETIME())

	declare @OrderStatusHistoryId NVARCHAR(max) = NEWID()
	insert into OrderStatusHistory(Id, CreatedDate, OrderId, OrderStatusId)
	values(@OrderStatusHistoryId, SYSDATETIME(), @OrderId, 1)

	select OrderId = @OrderId

END

----------------- 4 -----------------------------

CREATE PROCEDURE [dbo].[p_GetAllGoods]
AS
BEGIN
	SELECT Id = s.Id,
		   Price = s.Price,
		   Title = s.Title
	from Goods s
END

----------------- 5 -----------------------------
CREATE PROCEDURE [dbo].[p_GetBasket]
	@BasketId NVARCHAR(max)
AS
BEGIN
	select GoodId = s.GoodId,
		   Quantity = s.Quantity,
		   Title = g.Title,
		   Amount = g.Price * s.Quantity
	from Quantity s 
	join Goods g on g.Id = s.GoodId
	where s.BasketId = @BasketId
END

----------------- 6 -----------------------------
CREATE PROCEDURE [dbo].[p_GetOrderDetails]
	@OrderId NVARCHAR(max)
AS
BEGIN
	select BasketId = o.BasketId,
		   Address = d.Address,
		   CardNumber = d.CardNumber
	from Orders o
	join OrdersDetails d on d.OrderId = o.Id
	where o.Id = @OrderId
END
----------------- 7 -----------------------------
CREATE PROCEDURE [dbo].[p_GetOrderDetailsHistory]
	@OrderId NVARCHAR(max)
AS
BEGIN
	select StatusName = s.Description,
		   Date = h.CreatedDate
	from OrderStatusHistory h
	join OrderStatus s on s.Code = h.OrderStatusId
	where h.OrderId = @OrderId
END

----------------- 8 -----------------------------

CREATE PROCEDURE [dbo].[p_GetOrders]
AS
BEGIN
	select OrderId = o.OrderId,
		   CreatedDate = o.CreatedDate,
		   StatusName = s.Description
	from OrderStatusHistory o
	join OrderStatus s on s.Code = o.OrderStatusId
END

----------------- 9 -----------------------------

CREATE PROCEDURE [dbo].[p_UpdateOrderStatus]
	@OrderId NVARCHAR(max), 
	@StatusCode INT
AS
BEGIN
	insert into OrderStatusHistory(Id, CreatedDate, OrderId, OrderStatusId)
	values (NEWID(), SYSDATETIME(), @OrderId, @StatusCode)
END

