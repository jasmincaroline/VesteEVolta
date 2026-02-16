create table tb_user (
   id uuid primary key default gen_random_uuid(),
   name varchar(256) not null,
   telephone varchar(30),
   email varchar(256) unique not null,
   password varchar(256) not null,
   reported boolean not null default false,
   profile_type varchar(20) not null,
   created_at timestamptz not null default now(),
   constraint ck_user_profile_type
       check (profile_type in ('User','Owner'))
	);
create table tb_owner (
   user_id uuid primary key,
   constraint fk_owner_user
       foreign key (user_id)
       references tb_user(id)
);
create table tb_customer (
   user_id uuid primary key,
   constraint fk_customer_user
       foreign key (user_id)
       references tb_user(id)
);
create table tb_clothing (
   id uuid primary key default gen_random_uuid(),
   description varchar(500) not null,
   rent_price numeric(10,2) not null,
   availability_status varchar(30) not null,
   owner_id uuid not null,
   created_at timestamptz not null default now(),
   constraint fk_clothing_owner
       foreign key (owner_id)
       references tb_owner(user_id)
);
create table tb_category (
   category_id uuid primary key default gen_random_uuid(),
   name varchar(120) not null unique
);
create table tb_clothing_category (
   clothing_id uuid not null,
   category_id uuid not null,
   primary key (clothing_id, category_id),
   constraint fk_clothing_category_clothing
       foreign key (clothing_id)
       references tb_clothing(id),
   constraint fk_clothing_category_category
       foreign key (category_id)
       references tb_category(category_id)
);
create table tb_rental(
	id uuid primary key default gen_random_uuid(),
	user_id uuid not null,
	clothing_id uuid not null,
	
	start_date date not null,
	 end_date date not null,
   total_value numeric(10,2) not null,
   status varchar(20) not null,
   created_at timestamptz not null default now(),
   constraint fk_rental_user
       foreign key (user_id)
       references tb_user(id),
   constraint fk_rental_clothing
       foreign key (clothing_id)
       references tb_clothing(id),
   constraint ck_rental_status
       check (status in ('ongoing','active','finished','canceled')),
   constraint ck_rental_dates
       check (end_date >= start_date)
);
create table tb_rating (
   id uuid primary key default gen_random_uuid(),
   user_id uuid not null,
   rent_id uuid not null,
   clothing_id uuid not null,
   rating int not null,
   comment varchar(300),
   date date not null,
   created_at timestamptz not null default now(),
   constraint fk_rating_user
       foreign key (user_id)
       references tb_user(id),
   constraint fk_rating_rental
       foreign key (rent_id)
       references tb_rental(id),
   constraint fk_rating_clothing
       foreign key (clothing_id)
       references tb_clothing(id),
   constraint ck_rating_value
       check (rating between 1 and 5)
);
create table tb_report (
   report_id uuid primary key default gen_random_uuid(),
   reporter_id uuid not null,
   reported_id uuid not null,
   reported_clothing_id uuid not null,
   rental_id uuid,
   type varchar(50) not null,
   date date not null,
   reason varchar(120) not null,
   description varchar(800),
   status varchar(30) not null,
   created_at timestamptz not null default now(),
   constraint fk_report_reporter
       foreign key (reporter_id)
       references tb_user(id),
   constraint fk_report_reported
       foreign key (reported_id)
       references tb_user(id),
   constraint fk_report_clothing
       foreign key (reported_clothing_id)
       references tb_clothing(id),
   constraint fk_report_rental
       foreign key (rental_id)
       references tb_rental(id),
   constraint ck_report_users_different
       check (reporter_id <> reported_id)
);
create table tb_payment (
  id uuid primary key default gen_random_uuid(),
  rental_id uuid not null,
  payment_method varchar(30) not null,
  amount numeric(10,2) not null,
  payment_status varchar(20) not null default 'pending',
  payment_date timestamptz not null default now(),
  constraint
ck_payment_method check (payment_method in ('pix', 'credit_card', 'debit_card', 'boleto')),
  constraint
ck_payment_status check (payment_status in ('pending', 'paid', 'failed', 'refunded')),
  constraint fk_payment_rental
      foreign key (rental_id)
      references tb_rental(id)
);
--USER
INSERT INTO tb_user (name, telephone, email, password, profile_type)
VALUES
('Mariana Lopes', '(21) 99845-1123', 'mariana.lopes@email.com', 'hash_owner_01', 'Owner'),
('Camila Ferreira', '(11) 98734-5566', 'camila.ferreira@email.com', 'hash_owner_02', 'Owner'),
('Renata Azevedo', '(31) 99672-8899', 'renata.azevedo@email.com', 'hash_owner_03', 'Owner'),
('Juliana Martins', '(41) 99561-2244', 'juliana.martins@email.com', 'hash_owner_04', 'Owner'),
('Patricia Nogueira', '(51) 99345-7788', 'patricia.nogueira@email.com', 'hash_owner_05', 'Owner');
INSERT INTO tb_owner (user_id)
SELECT id
FROM tb_user
WHERE profile_type = 'Owner';
INSERT INTO tb_customer (user_id)
SELECT id
FROM tb_user
WHERE profile_type = 'User';
INSERT INTO tb_owner (user_id)
SELECT id FROM tb_user WHERE profile_type = 'Owner'
ON CONFLICT DO NOTHING;
INSERT INTO tb_customer (user_id)
SELECT id FROM tb_user WHERE profile_type = 'User'
ON CONFLICT DO NOTHING;
INSERT INTO tb_user (name, telephone, email, password, profile_type)
VALUES
('Cliente Teste', '(11) 90000-0001', 'cliente.teste@email.com', 'hash_user_teste', 'User'),
('Dono Teste', '(11) 90000-0002', 'dono.teste@email.com', 'hash_owner_teste', 'Owner')
ON CONFLICT (email) DO NOTHING;
--CLOTHING
INSERT INTO tb_clothing (description, rent_price, availability_status, owner_id)
VALUES
(
 'Vestido longo de festa vermelho, tecido acetinado, tamanho M',
 220.00,
 'AVAILABLE',
 (SELECT user_id FROM tb_owner ORDER BY random() LIMIT 1)
),
(
 'Vestido de gala preto, corte sereia, tamanho G',
 320.00,
 'RENTED',
 (SELECT user_id FROM tb_owner ORDER BY random() LIMIT 1)
),
(
 'Bolsa clutch dourada para eventos noturnos',
 90.00,
 'AVAILABLE',
 (SELECT user_id FROM tb_owner ORDER BY random() LIMIT 1)
),
(
 'Sapato social feminino salto alto nude, tamanho 37',
 75.00,
 'AVAILABLE',
 (SELECT user_id FROM tb_owner ORDER BY random() LIMIT 1)
),
(
 'Terno masculino slim fit azul marinho, tamanho 42',
 180.00,
 'RENTED',
 (SELECT user_id FROM tb_owner ORDER BY random() LIMIT 1)
),
(
 'Vestido casual midi floral, ideal para eventos diurnos',
 95.00,
 'AVAILABLE',
 (SELECT user_id FROM tb_owner ORDER BY random() LIMIT 1)
),
(
 'Bolsa estruturada preta em couro sintético',
 110.00,
 'MAINTENANCE',
 (SELECT user_id FROM tb_owner ORDER BY random() LIMIT 1)
),
(
 'Sapato masculino social preto, tamanho 41',
 70.00,
 'AVAILABLE',
 (SELECT user_id FROM tb_owner ORDER BY random() LIMIT 1)
);
INSERT INTO tb_clothing (description, rent_price, availability_status, owner_id)
SELECT
 x.description,
 x.rent_price,
 x.availability_status,
 o.user_id
FROM (SELECT user_id FROM tb_owner LIMIT 1) o
JOIN (VALUES
 ('Vestido longo de festa azul, tamanho M', 220.00, 'AVAILABLE'),
 ('Bolsa clutch dourada para eventos', 90.00, 'AVAILABLE'),
 ('Sapato social masculino preto, tamanho 41', 70.00, 'AVAILABLE')
) AS x(description, rent_price, availability_status)
ON true;
--CATEGORY
INSERT INTO tb_category (name)
VALUES
('Vestidos de Festa'),
('Vestidos Casuais'),
('Ternos'),
('Blazers'),
('Calças'),
('Saias'),
('Camisas'),
('Jaquetas'),
('Bolsas'),
('Sapatos'),
('Acessórios'),
('Moda Praia')
ON CONFLICT (name) DO NOTHING;
-- Vestidos de Festa (ex.: "festa", "gala")
INSERT INTO tb_clothing_category (clothing_id, category_id)
SELECT c.id, cat.category_id
FROM tb_clothing c
JOIN tb_category cat ON cat.name = 'Vestidos de Festa'
WHERE c.description ILIKE '%festa%' OR c.description ILIKE '%gala%';
-- Vestidos Casuais (ex.: "casual", "midi")
INSERT INTO tb_clothing_category (clothing_id, category_id)
SELECT c.id, cat.category_id
FROM tb_clothing c
JOIN tb_category cat ON cat.name = 'Vestidos Casuais'
WHERE c.description ILIKE '%casual%' OR c.description ILIKE '%midi%';
-- Ternos
INSERT INTO tb_clothing_category (clothing_id, category_id)
SELECT c.id, cat.category_id
FROM tb_clothing c
JOIN tb_category cat ON cat.name = 'Ternos'
WHERE c.description ILIKE '%terno%';
-- Bolsas
INSERT INTO tb_clothing_category (clothing_id, category_id)
SELECT c.id, cat.category_id
FROM tb_clothing c
JOIN tb_category cat ON cat.name = 'Bolsas'
WHERE c.description ILIKE '%bolsa%' OR c.description ILIKE '%clutch%';
-- Sapatos
INSERT INTO tb_clothing_category (clothing_id, category_id)
SELECT c.id, cat.category_id
FROM tb_clothing c
JOIN tb_category cat ON cat.name = 'Sapatos'
WHERE c.description ILIKE '%sapato%';
-- (Opcional) Acessórios (ex.: clutch também pode cair aqui)
INSERT INTO tb_clothing_category (clothing_id, category_id)
SELECT c.id, cat.category_id
FROM tb_clothing c
JOIN tb_category cat ON cat.name = 'Acessórios'
WHERE c.description ILIKE '%clutch%';
--RENTAL
INSERT INTO tb_rental (user_id, clothing_id, start_date, end_date, total_value, status)
SELECT
 u.id AS user_id,
 c.id AS clothing_id,
 s.start_date,
 (s.start_date + s.days - 1) AS end_date,
 ROUND((s.days * c.rent_price), 2) AS total_value,
 'finished' AS status
FROM (
 SELECT
   id,
   (current_date - (15 + (random()*90)::int))::date AS start_date,
   (1 + (random()*6)::int) AS days
 FROM tb_user
 WHERE profile_type = 'User'
 ORDER BY random()
 LIMIT 8
) s
JOIN tb_user u ON u.id = s.id
JOIN LATERAL (
 SELECT *
 FROM tb_clothing
 WHERE availability_status IN ('AVAILABLE','RENTED')
 ORDER BY random()
 LIMIT 1
) c ON true;
INSERT INTO tb_rental (user_id, clothing_id, start_date, end_date, total_value, status)
SELECT
 u.id,
 c.id,
 s.start_date,
 (s.start_date + s.days - 1) AS end_date,
 ROUND((s.days * c.rent_price), 2) AS total_value,
 (CASE WHEN random() < 0.5 THEN 'active' ELSE 'ongoing' END) AS status
FROM (
 SELECT
   id,
   (current_date - (0 + (random()*5)::int))::date AS start_date,
   (2 + (random()*7)::int) AS days
 FROM tb_user
 WHERE profile_type = 'User'
 ORDER BY random()
 LIMIT 5
) s
JOIN tb_user u ON u.id = s.id
JOIN LATERAL (
 SELECT *
 FROM tb_clothing
 WHERE availability_status = 'AVAILABLE'
 ORDER BY random()
 LIMIT 1
) c ON true;
INSERT INTO tb_rental (user_id, clothing_id, start_date, end_date, total_value, status)
SELECT
   u.id,
   c.id,
   current_date - 7,
   current_date - 4,
   ROUND(4 * c.rent_price, 2),
   'finished'
FROM (SELECT id FROM tb_user WHERE profile_type = 'User' ORDER BY random() LIMIT 1) u
JOIN (SELECT id, rent_price FROM tb_clothing ORDER BY random() LIMIT 3) c
ON true;
-- PAYMENT
INSERT INTO tb_payment (rental_id, payment_method, amount, payment_status, payment_date)
SELECT
 r.id AS rental_id,
 (ARRAY['pix','credit_card','debit_card','boleto'])[1 + floor(random()*4)] AS payment_method,
 r.total_value AS amount,
 CASE
   WHEN r.status = 'finished' THEN 'paid'
   WHEN r.status IN ('active','ongoing') THEN (CASE WHEN random() < 0.7 THEN 'paid' ELSE 'pending' END)
   WHEN r.status = 'canceled' THEN (CASE WHEN random() < 0.6 THEN 'refunded' ELSE 'failed' END)
   ELSE 'pending'
 END AS payment_status,
 (r.created_at + (random() * interval '3 days')) AS payment_date
FROM tb_rental r
LEFT JOIN tb_payment p ON p.rental_id = r.id
WHERE p.rental_id IS NULL;
INSERT INTO tb_payment (rental_id, payment_method, amount, payment_status)
SELECT
   r.id,
   'pix',
   r.total_value,
   'paid'
FROM tb_rental r;
INSERT INTO tb_payment (rental_id, payment_method, amount, payment_status)
SELECT r.id, 'pix', r.total_value, 'paid'
FROM tb_rental r
LEFT JOIN tb_payment p ON p.rental_id = r.id
WHERE p.rental_id IS NULL;
--RATING
INSERT INTO tb_rating (user_id, rent_id, clothing_id, rating, comment, date)
SELECT
 r.user_id,
 r.id AS rent_id,
 r.clothing_id,
 (CASE
    WHEN random() < 0.10 THEN 1
    WHEN random() < 0.25 THEN 2
    WHEN random() < 0.55 THEN 3
    WHEN random() < 0.85 THEN 4
    ELSE 5
  END) AS rating,
 (CASE
    WHEN random() < 0.20 THEN 'Tive um problema com o tamanho, mas a peça estava ok.'
    WHEN random() < 0.40 THEN 'Peça bonita e bem cuidada. Recomendo.'
    WHEN random() < 0.60 THEN 'Boa experiência, entrega e devolução tranquilas.'
    WHEN random() < 0.80 THEN 'Veio impecável, cheirosa e bem embalada.'
    ELSE 'Qualidade excelente, valeu muito a pena!'
  END) AS comment,
 (r.end_date + (1 + (random()*7)::int))::date AS date
FROM tb_rental r
LEFT JOIN tb_rating rt ON rt.rent_id = r.id
WHERE r.status = 'finished'
 AND rt.rent_id IS NULL
ORDER BY random()
LIMIT 10;
--REPORT
INSERT INTO tb_report
(reporter_id, reported_id, reported_clothing_id, rental_id, type, date, reason, description, status)
SELECT
 r.user_id AS reporter_id,
 c.owner_id AS reported_id,
 r.clothing_id AS reported_clothing_id,
 r.id AS rental_id,
 (ARRAY['item_condition','late_delivery','misleading_description','other'])[1 + floor(random()*4)] AS type,
 (r.end_date + (random()*5)::int)::date AS date,
 (ARRAY[
   'Peça com sinais de uso acima do esperado',
   'Entrega atrasada ou dificuldade de retirada',
   'Descrição não corresponde ao produto recebido',
   'Divergência de tamanho/medidas informadas',
   'Problemas na comunicação'
 ])[1 + floor(random()*5)] AS reason,
 (ARRAY[
   'A peça chegou com pequenos danos e não estava como descrito no anúncio.',
   'Tive dificuldade para combinar retirada/devolução e não obtive retorno rápido.',
   'O produto não correspondia às fotos, e isso impactou meu evento.',
   'Houve divergência de tamanho e não consegui usar conforme planejado.',
   'Solicito análise e orientação sobre como proceder.'
 ])[1 + floor(random()*5)] AS description,
 (ARRAY['open','under_review','resolved','rejected'])[1 + floor(random()*4)] AS status
FROM tb_rental r
JOIN tb_clothing c ON c.id = r.clothing_id
LEFT JOIN tb_report rp ON rp.rental_id = r.id
WHERE rp.rental_id IS NULL
ORDER BY random()
LIMIT 6;
INSERT INTO tb_report
(reporter_id, reported_id, reported_clothing_id, rental_id, type, date, reason, description, status)
SELECT
 u.id AS reporter_id,
 c.owner_id AS reported_id,
 c.id AS reported_clothing_id,
 NULL::uuid AS rental_id,
 (ARRAY['listing_issue','inappropriate_content','fraud_suspicion','other'])[1 + floor(random()*4)] AS type,
 (current_date - (random()*30)::int)::date AS date,
 (ARRAY[
   'Anúncio com informações insuficientes',
   'Suspeita de anúncio enganoso',
   'Conteúdo inadequado nas fotos/descrição',
   'Tentativa de negociação fora da plataforma',
   'Outro'
 ])[1 + floor(random()*5)] AS reason,
 (ARRAY[
   'Solicito verificação do anúncio, pois há inconsistências nas informações apresentadas.',
   'Acredito que o anúncio pode induzir o cliente ao erro. Peço revisão.',
   'Percebi tentativa de negociação fora do sistema. Gostaria que a equipe analisasse.',
   'As informações parecem não condizer com o produto. Peço análise.',
   'Estou reportando por precaução e para manter a segurança da plataforma.'
 ])[1 + floor(random()*5)] AS description,
 (ARRAY['open','under_review','resolved'])[1 + floor(random()*3)] AS status
FROM tb_user u
JOIN tb_clothing c ON true
LEFT JOIN tb_report rp
 ON rp.reporter_id = u.id
AND rp.reported_clothing_id = c.id
WHERE u.profile_type = 'User'
 AND u.id <> c.owner_id
ORDER BY random()
LIMIT 4;
--Testes
SELECT
 u.name,
 r.id AS rental_id,
 r.total_value
FROM tb_user u
JOIN tb_rental r ON r.user_id = u.id
JOIN tb_payment p ON p.rental_id = r.id
WHERE p.payment_method = 'pix';

