-- ============================================================
-- InventoryMS Database Schema (PostgreSQL)
-- ============================================================

-- ---------- Enumerated types ----------
CREATE TYPE transaction_type AS ENUM ('StockIn', 'StockOut', 'Transfer', 'Adjustment');
CREATE TYPE order_status     AS ENUM ('Pending', 'Received', 'Cancelled');

-- ---------- Identity / Users & Roles ----------
CREATE TABLE roles (
    id   UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE users (
    id             UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    full_name      VARCHAR(150) NOT NULL,
    email          VARCHAR(150) NOT NULL UNIQUE,
    password_hash  TEXT NOT NULL,
    is_active      BOOLEAN NOT NULL DEFAULT TRUE,
    created_at     TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE user_roles (
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    role_id UUID NOT NULL REFERENCES roles(id) ON DELETE CASCADE,
    PRIMARY KEY (user_id, role_id)
);

-- ---------- Catalog ----------
CREATE TABLE categories (
    id          SERIAL PRIMARY KEY,
    name        VARCHAR(100) NOT NULL,
    description VARCHAR(500)
);

CREATE TABLE suppliers (
    id           SERIAL PRIMARY KEY,
    name         VARCHAR(150) NOT NULL,
    contact_name VARCHAR(100),
    email        VARCHAR(100),
    phone        VARCHAR(30),
    address      VARCHAR(250)
);

CREATE TABLE warehouses (
    id       SERIAL PRIMARY KEY,
    name     VARCHAR(100) NOT NULL,
    location VARCHAR(250)
);

CREATE TABLE products (
    id             SERIAL PRIMARY KEY,
    sku            VARCHAR(40)  NOT NULL UNIQUE,
    name           VARCHAR(150) NOT NULL,
    description    VARCHAR(1000),
    category_id    INTEGER REFERENCES categories(id) ON DELETE SET NULL,
    supplier_id    INTEGER REFERENCES suppliers(id) ON DELETE SET NULL,
    unit_price     NUMERIC(18,2) NOT NULL DEFAULT 0 CHECK (unit_price >= 0),
    reorder_level  INTEGER NOT NULL DEFAULT 10 CHECK (reorder_level >= 0),
    created_at     TIMESTAMPTZ NOT NULL DEFAULT now()
);

-- ---------- Stock ----------
CREATE TABLE inventory_stock (
    id           SERIAL PRIMARY KEY,
    product_id   INTEGER NOT NULL REFERENCES products(id)   ON DELETE CASCADE,
    warehouse_id INTEGER NOT NULL REFERENCES warehouses(id) ON DELETE CASCADE,
    quantity     INTEGER NOT NULL DEFAULT 0 CHECK (quantity >= 0),
    UNIQUE (product_id, warehouse_id)
);

CREATE TABLE stock_transactions (
    id              SERIAL PRIMARY KEY,
    product_id      INTEGER NOT NULL REFERENCES products(id)   ON DELETE RESTRICT,
    warehouse_id    INTEGER NOT NULL REFERENCES warehouses(id) ON DELETE RESTRICT,
    to_warehouse_id INTEGER REFERENCES warehouses(id) ON DELETE RESTRICT, -- used for Transfer only
    type            transaction_type NOT NULL,
    quantity        INTEGER NOT NULL CHECK (quantity > 0),
    date            TIMESTAMPTZ NOT NULL DEFAULT now(),
    reference       VARCHAR(100),
    notes           VARCHAR(500),
    user_id         UUID REFERENCES users(id) ON DELETE SET NULL
);

-- ---------- Purchasing ----------
CREATE TABLE purchase_orders (
    id            SERIAL PRIMARY KEY,
    supplier_id   INTEGER NOT NULL REFERENCES suppliers(id)  ON DELETE RESTRICT,
    warehouse_id  INTEGER NOT NULL REFERENCES warehouses(id) ON DELETE RESTRICT,
    order_date    TIMESTAMPTZ NOT NULL DEFAULT now(),
    received_date TIMESTAMPTZ,
    status        order_status NOT NULL DEFAULT 'Pending'
);

CREATE TABLE purchase_order_items (
    id                SERIAL PRIMARY KEY,
    purchase_order_id INTEGER NOT NULL REFERENCES purchase_orders(id) ON DELETE CASCADE,
    product_id        INTEGER NOT NULL REFERENCES products(id)        ON DELETE RESTRICT,
    quantity          INTEGER NOT NULL CHECK (quantity > 0),
    unit_cost         NUMERIC(18,2) NOT NULL DEFAULT 0 CHECK (unit_cost >= 0)
);

-- ---------- Indexes ----------
CREATE INDEX idx_products_category          ON products(category_id);
CREATE INDEX idx_products_supplier          ON products(supplier_id);
CREATE INDEX idx_inventory_stock_product    ON inventory_stock(product_id);
CREATE INDEX idx_inventory_stock_warehouse  ON inventory_stock(warehouse_id);
CREATE INDEX idx_stock_txn_product          ON stock_transactions(product_id);
CREATE INDEX idx_stock_txn_warehouse        ON stock_transactions(warehouse_id);
CREATE INDEX idx_stock_txn_date             ON stock_transactions(date);
CREATE INDEX idx_po_supplier                ON purchase_orders(supplier_id);
CREATE INDEX idx_po_status                  ON purchase_orders(status);
CREATE INDEX idx_poi_purchase_order         ON purchase_order_items(purchase_order_id);

-- ---------- Seed roles ----------
INSERT INTO roles (name) VALUES ('Admin'), ('Manager'), ('Staff');
