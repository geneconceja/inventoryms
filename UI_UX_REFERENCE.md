# InventoryMS — UI & UX Design Reference

> **Purpose**: This file is the single source of truth for all visual and interaction decisions in InventoryMS.
> Every new view, partial, or component **must** follow these rules to maintain a consistent, professional look.

---

## 1. Brand Color Palette

| Token (CSS Variable) | Hex | Usage |
|---|---|---|
| `--brand-deep-marine` | `#146C94` | Sidebar background, primary buttons, card header text, authoritative accents |
| `--brand-cerulean` | `#19A7CE` | Button hover, active sidebar indicator, focus rings, accent borders |
| `--brand-powder-blue` | `#AFD3E2` | Table row hover, badge backgrounds, dark-surface subtext, icon tints |
| `--brand-canvas` | `#F6F1F1` | Main page canvas background, auth hint background |

### Supporting Tokens

| Token | Hex | Usage |
|---|---|---|
| `--primary` | `#146C94` | Maps to deep marine — default for all interactive elements |
| `--primary-dark` | `#0E4B66` | Sidebar brand area, pressed button states |
| `--primary-light` | `#19A7CE` | Maps to cerulean — hover, focus |
| `--surface` | `#FFFFFF` | Card bodies, modal backgrounds, input backgrounds |
| `--surface-alt` | `#F6F1F1` | Page canvas, table headers |
| `--border` | `#D8DEE6` | Standard 1px dividers between containers |
| `--border-dark` | `#C4D3DC` | Input borders, stronger separators |
| `--text` | `#0E2F44` | Primary body text — high contrast dark ink |
| `--text-muted` | `#4F6B7A` | Secondary text, captions, metadata |

### Status / Semantic Colors (always use solid, no gradients)

| State | Foreground | Background | Border | Usage |
|---|---|---|---|---|
| **Success** | `#15803D` | `#DCFCE7` | `#86EFAC` | In Stock, Received, Active |
| **Warning** | `#B45309` | `#FEF3C7` | `#FDE68A` | Low Stock, Pending |
| **Danger** | `#B91C1C` | `#FEE2E2` | `#FECACA` | Out of Stock, Cancelled, Errors |
| **Info** | `#146C94` | `#AFD3E2` | `#AFD3E2` | Transfers, Drafts, Neutral info |

---

## 2. Typography

### Font Families
- **UI Text**: `Inter`, -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif
- **Numerics/Codes**: `JetBrains Mono`, Consolas, `SF Mono`, monospace

> Use **Inter** for all labels, titles, body text, and navigation.
> Use **JetBrains Mono** for SKUs, barcodes, quantities, prices, order numbers, and any numeric data in tables. Apply class `font-mono` to the element.

### Type Scale

| Role | Size | Weight | Class / Element |
|---|---|---|---|
| Page Title (H1) | `1.35rem` | 700 | `.page-header-left h1` |
| Card / Section Header | `0.92rem` | 700 + UPPERCASE | `.card-header-title` |
| Body Default | `1rem` (16px) | 400 | `body` |
| Sidebar Nav Link | `0.975rem` | 500 | `.sidebar-link` |
| Section Label | `0.72rem` | 700 + UPPERCASE | `.sidebar-section-label` |
| Table Header | `0.72rem` | 700 + UPPERCASE | `thead th` |
| Badges / Tags | `0.7rem` | 700 + UPPERCASE | `.badge-role`, `.badge-status` |
| Stat Value | `1.65rem` | 700 | `.stat-value` (JetBrains Mono) |
| Stat Label | `0.78rem` | 600 + UPPERCASE | `.stat-label` |
| Caption / Muted | `0.85rem` | 400 | Footer text, dev hints |

---

## 3. Geometry & Spacing Rules

### Strict: Zero Border Radius
```
All elements MUST have border-radius: 0px — sharp 90-degree corners everywhere.
No exceptions for cards, buttons, badges, inputs, modals, dropdowns, or alerts.
```

### Strict: No Gradients
```
Never use linear-gradient(), radial-gradient(), or any multi-stop background.
All backgrounds must be flat, solid hex colors or CSS variables.
```

### Spacing Scale (multiples of 4px)
| Value | Pixels | Usage |
|---|---|---|
| `0.25rem` | 4px | Tight inline spacing |
| `0.5rem` | 8px | Badge padding, small gaps |
| `0.75rem` | 12px | Nav icon gap, sidebar padding |
| `1rem` | 16px | Standard gap, card body inner |
| `1.25rem` | 20px | Card padding |
| `1.5rem` | 24px | Section margins |
| `1.75rem` | 28px | Page content padding |
| `2rem` | 32px | Auth card body |

### Layout Dimensions
| Element | Value |
|---|---|
| Sidebar width | `290px` |
| Main content offset | `margin-left: 290px` |
| Topbar height | `60px` |
| Page content padding | `1.75rem` |
| Table row height | ~44px (cell padding: `0.75rem 1rem`) |
| Login card max-width | `520px` |

---

## 4. Component Patterns

### 4.1 Buttons

Always use Bootstrap button classes extended by `site.css`. Never add inline `background`, `color`, or `border-radius`.

| Class | Background | Hover Background |
|---|---|---|
| `.btn.btn-primary` | `#146C94` | `#19A7CE` |
| `.btn.btn-outline-primary` | Transparent | `#AFD3E2` |
| `.btn.btn-secondary` | `#4F6B7A` | `#38505C` |
| `.btn.btn-outline-secondary` | `#FFFFFF` | `#F6F1F1` |
| `.btn.btn-danger` | `#B91C1C` | Darker red |

```html
<!-- Primary action -->
<a asp-action="Create" class="btn btn-primary" id="btn-create-product">
    <i class="fas fa-plus me-2"></i>New Product
</a>

<!-- Secondary / Cancel -->
<a asp-action="Index" class="btn btn-outline-secondary">
    <i class="fas fa-arrow-left me-2"></i>Back
</a>
```

### 4.2 Page Header (Required on Every Page)

```html
<div class="page-header fade-in">
    <div class="page-header-left">
        <h1>Page Title</h1>
        <p>Short description of this section.</p>
    </div>
    <a asp-action="Create" class="btn btn-primary" id="btn-create-XXX">
        <i class="fas fa-plus me-2"></i>New Item
    </a>
</div>
```

### 4.3 Cards

```html
<div class="card fade-in">
    <div class="card-header">
        <h2 class="card-header-title">All Products</h2>
        <span class="text-muted" style="font-size:0.85rem;">42 records</span>
    </div>
    <div class="card-body">
        <!-- content here -->
    </div>
</div>
```

### 4.4 Stat / KPI Cards

```html
<div class="stat-card">
    <div class="stat-icon primary">
        <i class="fas fa-boxes-stacked"></i>
    </div>
    <div>
        <div class="stat-value">1,284</div>
        <div class="stat-label">Total Products</div>
    </div>
</div>
```

Available `.stat-icon` modifiers: `.primary` `.success` `.warning` `.danger` `.accent`

### 4.5 Tables

```html
<div class="table-wrapper">
    <table class="table" id="products-table">
        <thead>
            <tr>
                <th>SKU</th>
                <th>Name</th>
                <th>Qty</th>
                <th>Status</th>
                <th style="width:120px;">Actions</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td class="font-mono">SKU-00123</td>
                <td>Product Name</td>
                <td class="font-mono">150</td>
                <td><span class="badge-status active">Active</span></td>
                <td>
                    <a asp-action="Edit" asp-route-id="@item.Id" class="btn btn-outline-primary btn-sm">Edit</a>
                </td>
            </tr>
        </tbody>
    </table>
</div>
```

### 4.6 Badges

```html
<!-- Role badges -->
<span class="badge-role admin">Admin</span>
<span class="badge-role manager">Manager</span>
<span class="badge-role staff">Staff</span>

<!-- Status badges -->
<span class="badge-status active">Active</span>
<span class="badge-status inactive">Inactive</span>

<!-- Order status -->
<span class="badge-order-status pending">Pending</span>
<span class="badge-order-status received">Received</span>
<span class="badge-order-status cancelled">Cancelled</span>
```

### 4.7 Forms

```html
<div class="mb-3">
    <label asp-for="FieldName" class="form-label">Field Label</label>
    <input asp-for="FieldName" class="form-control" placeholder="..." />
    <span asp-validation-for="FieldName" class="text-danger" style="font-size:0.85rem;"></span>
</div>

<div class="mb-3">
    <label asp-for="CategoryId" class="form-label">Category</label>
    <select asp-for="CategoryId" class="form-select" asp-items="Model.Categories">
        <option value="">— Select Category —</option>
    </select>
</div>
```

### 4.8 Alerts

```html
<div class="alert alert-success">
    <i class="fas fa-check-circle me-2"></i>Record saved successfully.
</div>

<div class="alert alert-danger">
    <i class="fas fa-exclamation-circle me-2"></i>Operation failed.
</div>
```

Available: `.alert-success` `.alert-danger` `.alert-warning` `.alert-info`

### 4.9 Search Bars

```html
<form method="get" class="mb-4 d-flex gap-2" style="max-width:400px;">
    <input type="text" name="search" value="@ViewBag.Search"
           class="form-control search-input"
           id="product-search"
           placeholder="Search by name or SKU..." />
    <button type="submit" class="btn btn-outline-primary" id="btn-search">
        <i class="fas fa-magnifying-glass"></i>
    </button>
</form>
```

---

## 5. Layout Rules

### Controller Actions
Every action that returns a view must set:
```csharp
ViewData["Title"] = "Page Name";         // Browser tab title
ViewData["PageTitle"] = "Page Name";     // Topbar heading
```

### Required Page Structure
```html
@{
    ViewData["Title"] = "Products";
    ViewData["PageTitle"] = "Products";
}

<!-- 1. Page header -->
<div class="page-header fade-in">...</div>

<!-- 2. Optional: search / filters -->

<!-- 3. Main content card -->
<div class="card fade-in">...</div>
```

### Do's ✅
- Add `fade-in` class to the top-level content div for smooth page entry
- Add `id` attributes to every table, button, form, and search input
- Use `font-mono` class on all numeric/code table cells
- Use CSS variables for all colors: `var(--brand-deep-marine)`, `var(--border)`, etc.
- Keep row actions in the last column with a fixed width (`style="width:120px;"`)

### Don'ts ❌
- ❌ No `border-radius` on any element
- ❌ No `linear-gradient()` or any gradient background
- ❌ No hardcoded hex colors inline — use CSS variables
- ❌ No heavy `box-shadow` — use `border: 1px solid var(--border)` instead
- ❌ No emoji as icons — use Font Awesome `<i class="fas fa-..."></i>`
- ❌ No Bootstrap rounded utilities (`rounded`, `rounded-pill`, etc.)

---

## 6. Icons (Font Awesome 6 Free)

| Context | Icon |
|---|---|
| Dashboard | `fa-chart-line` |
| Products | `fa-box` |
| Inventory / Stock | `fa-boxes-stacked` |
| Stock In | `fa-arrow-down` |
| Stock Out | `fa-arrow-up` |
| Transfer | `fa-arrows-left-right` |
| Purchase Orders | `fa-file-invoice` |
| Suppliers | `fa-truck` |
| Categories | `fa-tags` |
| Warehouses | `fa-warehouse` |
| Users | `fa-users` |
| Edit | `fa-pen-to-square` |
| Delete | `fa-trash` |
| Add / New | `fa-plus` |
| Search | `fa-magnifying-glass` |
| Save | `fa-floppy-disk` |
| Cancel / Back | `fa-arrow-left` |
| Settings | `fa-gear` |
| Warning | `fa-triangle-exclamation` |
| Info | `fa-circle-info` |
| Adjust | `fa-sliders` |

---

## 7. Naming Conventions

| Scope | Convention | Example |
|---|---|---|
| Controller actions | PascalCase | `Index`, `Create`, `Edit`, `ToggleStatus` |
| ViewData keys | Quoted strings | `"Title"`, `"PageTitle"` |
| HTML element IDs | kebab-case, context-prefixed | `btn-create-user`, `user-search`, `users-table` |
| CSS classes | kebab-case | `.badge-status`, `.stat-card` |
| View files | Match controller | `Products/Index.cshtml`, `Users/Create.cshtml` |

---

## 8. Quick Reference Cheatsheet

```
COLORS
  --brand-deep-marine  #146C94  Primary, sidebar, buttons
  --brand-cerulean     #19A7CE  Hover, focus, active
  --brand-powder-blue  #AFD3E2  Tints, icon color, row hover
  --brand-canvas       #F6F1F1  Page canvas, auth hints

GEOMETRY
  border-radius: 0px  (ALWAYS — no exceptions)
  No gradients        (EVER)
  Elevation: border: 1px solid var(--border)

TYPOGRAPHY
  UI font:   Inter (16px base)
  Data font: JetBrains Mono  →  class="font-mono"

LAYOUT
  Sidebar:  290px  |  BG: #146C94  |  Active: #0E4B66 + cerulean left-border
  Content:  margin-left: 290px  |  Padding: 1.75rem

BUTTONS
  Primary:   #146C94  →  hover: #19A7CE
  Outline:   border #146C94  →  hover: #AFD3E2 bg
  Secondary: #4F6B7A

FORMS
  Input border:  #C4D3DC
  Focus:         border + outline  #146C94 / #19A7CE

BADGES (rectangular, all-caps, 0px radius)
  Active / Success  →  #15803D on #DCFCE7
  Pending / Warning →  #B45309 on #FEF3C7
  Cancelled / Error →  #B91C1C on #FEE2E2
  Info / Transfer   →  #146C94 on #AFD3E2
```
