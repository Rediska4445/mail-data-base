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

