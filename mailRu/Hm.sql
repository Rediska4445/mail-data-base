use Mail;

-- Типография
create table printing_house (id int primary key not null, addr varchar(96));

-- Газетка
create table newspaper (id int primary key not null, title varchar(64), edition_code varchar(64), price int not null, full_name varchar(96) not null, foreign key (id) references printing_house(id), number int);

-- Почта (TODO: изменить main на mail)
create table main (id int primary key not null, addr varchar(96), foreign key (id) references newspaper(id), number_newspaper int);

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
SELECT
    ph.id AS printing_house_id,
    ph.addr AS printing_house_addr,
    np.title AS newspaper_title,
    np.edition_code,
    np.price,
    np.full_name,
    np.number,
    m.addr AS main_addr,
    m.number_newspaper
FROM
    printing_house ph
JOIN
    newspaper np ON np.id = ph.id
JOIN
    main m ON m.id = np.id;

-- Почта
SELECT 
    m.id,
    m.addr AS main_addr,
    m.number_newspaper,
    np.title AS newspaper_title
FROM main m
JOIN newspaper np ON m.id = np.id
