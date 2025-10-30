use Mail;

-- Типография
create table printing_house (id int primary key not null, addr varchar(96));

-- Газетка
create table newspaper (id int primary key not null, title varchar(64), edition_code varchar(64), price int not null, full_name varchar(96) not null, printing_house int null, foreign key (printing_house) references printing_house(id), number int);

-- Почта (TODO: изменить main на mail)
create table main (id int primary key not null, addr varchar(96), newspaper_id int, foreign key (newspaper_id) references newspaper(newspaper_id), number_newspaper int);

ALTER TABLE main DROP CONSTRAINT FK__main__id__29572725;

ALTER TABLE main ADD newspaper_id int NULL;

ALTER TABLE main
ADD CONSTRAINT FK_main_newspaper FOREIGN KEY (newspaper_id) REFERENCES newspaper(id);

-- Мок для типографии
INSERT INTO printing_house (id, addr) VALUES
(1, '123 Main St'),
(2, '456 Oak Ave'),
(3, '789 Pine Rd');

-- Мок для газетки 
INSERT INTO newspaper (id, title, edition_code, price, full_name, number) VALUES
(1, 'Daily News', 'DN001', 100, 'Daily News Full Name', 10),
(2, 'Morning Star', 'MS002', 120, 'Morning Star Full Name', 15),
(3, 'Evening Post', 'EP003', 110, 'Evening Post Full Name', 12);

-- Мок для почты
INSERT INTO main (id, addr, number_newspaper) VALUES
(1, 'Main Addr 1', 5),
(2, 'Main Addr 2', 8),
(3, 'Main Addr 3', 7);

-- Тесты
-- Газета
SELECT n.id, n.title, n.edition_code, n.price, n.full_name, n.printing_house, ph.addr AS printing_house_addr, n.number
FROM newspaper n
LEFT JOIN printing_house ph ON n.printing_house = ph.id;

-- Почта
SELECT m.id, m.addr, m.number_newspaper, m.newspaper_id, n.title AS newspaper_title
FROM main m
LEFT JOIN newspaper n ON m.newspaper_id = n.id;

-- Other

use Mail;

select * from newspaper;

SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'newspaper';

BEGIN TRANSACTION;

SELECT 
    fk.name AS foreign_key_name,
    tp.name AS parent_table,
    cp.name AS parent_column,
    tr.name AS referenced_table,
    cr.name AS referenced_column
FROM 
    sys.foreign_keys AS fk
INNER JOIN sys.foreign_key_columns AS fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.tables AS tp ON fk.parent_object_id = tp.object_id
INNER JOIN sys.columns AS cp ON fkc.parent_object_id = cp.object_id AND fkc.parent_column_id = cp.column_id
INNER JOIN sys.tables AS tr ON fk.referenced_object_id = tr.object_id
INNER JOIN sys.columns AS cr ON fkc.referenced_object_id = cr.object_id AND fkc.referenced_column_id = cr.column_id
WHERE 
    tp.name = 'newspaper';


UPDATE newspaper
SET printing_house = 3
WHERE id = 3;

--FK__main__id__29572725
ALTER TABLE newspaper DROP CONSTRAINT FK__newspaper__id__267ABA7A;

ALTER TABLE newspaper ADD printing_house int NULL;

ALTER TABLE newspaper
ADD CONSTRAINT FK_main_printing_house FOREIGN KEY (printing_house) REFERENCES printing_house(id);

INSERT INTO main (id, addr, number_newspaper, newspaper_id)
VALUES (12, 'ebanina', 12, 1);

UPDATE main
SET newspaper_id = 2
WHERE id = 2;

select * from newspaper;

UPDATE main
SET number_newspaper = 10
WHERE id = 2;

COMMIT TRANSACTION;

SELECT n.id, n.title, n.edition_code, n.price, n.full_name, n.printing_house, ph.addr AS printing_house_addr, n.number
FROM newspaper n
LEFT JOIN printing_house ph ON n.printing_house = ph.id;
