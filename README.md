# Audit Tracker - .NET 8 Blazor Application

## Table of Contents
1. [Project Overview](#project-overview)
2. [Technology Stack](#technology-stack)
3. [Prerequisites & Setup](#prerequisites--setup)
4. [Architecture Overview](#architecture-overview)
5. [Database Models](#database-models)
6. [Services & Business Logic](#services--business-logic)
7. [Authentication & Authorization](#authentication--authorization)
8. [Pages & Routes](#pages--routes)
9. [Components](#components)
10. [Configuration](#configuration)
11. [Running the Application](#running-the-application)
12. [Deployment](#deployment)
13. [Troubleshooting](#troubleshooting)

---

## Project Overview

**Audit Tracker** is an enterprise-grade .NET 8 Blazor interactive server application designed for inventory audit visualization and management across departments and sub-departments. The application enables organizations to track inventory audits by:

- Managing multiple audit inventories with real-time status tracking
- Organizing inventory across divisions (departments) and zones (sub-departments)
- Providing role-based access control for audit masters and auditors
- Visualizing audit progress with real-time updates
- Maintaining comprehensive audit records with creation/update timestamps

**Target Users:**
- Audit Masters: Can manage all master data, users, and inventories
- Auditors: Can view and update audit records within assigned inventories

---

## Technology Stack

### Core Framework
- **Runtime:** .NET 8.0
- **Web Framework:** ASP.NET Core Blazor (Interactive Server Rendering)
- **Language:** C# 12

### Database & ORM
- **Database:** MySQL 8.0  
- **ORM:** Entity Framework Core 8.0.13
- **Connection:** MySQL.EntityFrameworkCore 8.0.11

### UI Framework
- **Component Library:** MudBlazor 8.2.0 (Material Design UI components)
- **Layout:** MudLayout with AppBar, Drawer, and MainContent

### Authentication & Authorization
- **Authentication:** Windows Authentication (IIS Integration)
- **Authorization:** Custom Claims-based policies (Master, Auditor roles)
- **HTTP Context:** Integrated with AspNetCore.Components.Authorization

### Build & Deployment
- **Package Manager:** NuGet
- **Build Configuration:** Debug/Release for x86 and AnyCPU platforms
- **IIS Hosting:** Compatible with IIS Express and full IIS

---

## Prerequisites & Setup

### System Requirements
- **Operating System:** Windows 10/11 or Windows Server 2019+
- **Runtime:** .NET 8.0 SDK or later
- **Database:** MySQL 8.0 or compatible
- **IIS:** IIS Express (included with Visual Studio) or full IIS 10.0+
- **Visual Studio:** 2022 (17.8+) with ASP.NET and web development workload

### Step 1: Clone & Open Project
```bash
# Clone the repository
git clone <repository-url>

# Navigate to project directory
cd Audit_Tracker-DOTNET-Blazer

# Open in Visual Studio 2022
start Audit_Tracker-DOTNET.sln
```

### Step 2: Install Dependencies
```bash
# Restore NuGet packages (automatic in Visual Studio on load)
dotnet restore

# Or via Package Manager Console
Update-Package
```

### Step 3: Configure Database Connection
Edit [appsettings.Development.json](Audit_Tracker-DOTNET/appsettings.Development.json):
```json
{
  "ConnectionStrings": {
    "MySQLConection": "server=<host>;Port=3306;uid=<username>;pwd=<password>;database=aap_inventory_db_qa"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

**Example (Development):**
```
server=localhost;Port=3306;uid=root;pwd=password123;database=aap_inventory_db_qa
```

**Example (Production):**  
```
server=hmaapp350.am.mds.honda.com;Port=3306;uid=AAP_INV;pwd=P@SS4INV;database=aap_inventory_db_qa
```

### Step 4: Configure Windows Authentication
The application uses Windows domain authentication. Ensure:
1. **IIS/IIS Express is configured for Windows Authentication:**
   - In Visual Studio: Project Properties → Debug → Enable Windows Authentication
   - Disable Anonymous Authentication
   
2. **Active Directory Domain (DOM Domain):**
   - Default domain: `DOM\\`
   - Users: Extract username after domain prefix (e.g., `DOM\jsmith` → `jsmith`)

3. **Allowed SIDs (in [SD.cs](Audit_Tracker-DOTNET/SD.cs)):**
   ```csharp
   "S-1-5-21-4127812034-820336945-2256232113-513" // Domain Users
   ```

### Step 5: Create/Update Database
```bash
# Navigate to project directory
cd Audit_Tracker-DOTNET

# Create initial migrations (if needed)
dotnet ef migrations add initial --context InventoryDbContext
dotnet ef migrations add initial --context AuthenticationContext

# Update database with latest migrations
dotnet ef database update --context InventoryDbContext
dotnet ef database update --context AuthenticationContext
```

**Existing Migrations:**
- `Audit_Tracker-DOTNET/Migrations/20260305203506_init.cs` - Initial InventoryDbContext
- `Audit_Tracker-DOTNET/Migrations/InventoryDb/20260305203519_init.cs` - Initial InventoryDbContext (v2)
- `Audit_Tracker-DOTNET/Migrations/AuthenticationContextModelSnapshot.cs`
- `Audit_Tracker-DOTNET/Migrations/InventoryDb/InventoryDbContextModelSnapshot.cs`

### Step 6: Seed Master Data (Optional)
Create initial users and roles in the database:
```sql
-- Roles Table
INSERT INTO Roles (Code, Desc, CreatedDate, UpdatedDate) VALUES
('Master', 'System Administrator', NOW(), NOW()),
('Auditor', 'Audit Personnel', NOW(), NOW());

-- Audit_Admins Table (Users)
INSERT INTO Audit_Admins (Username, Name, RoleID, CreatedDate, UpdatedDate) VALUES
('jsmith', 'John Smith', 1, NOW(), NOW()),
('jdoe', 'Jane Doe', 2, NOW(), NOW());
```

---

## Architecture Overview

### Application Layers

```
┌─────────────────────────────────────────┐
│      Blazor Interactive UI Layer        │
│  (Pages, Components, MudBlazor)         │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│     Service Layer (Business Logic)      │
│  - DB_Services                          │
│  - Admin_Services                       │
├─────────────────────────────────────────┤
│  - Authentication Services              │
│  - Authorization Policies               │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│    Data Access Layer (EF Core DbSet)    │
│  - InventoryDbContext                   │
│  - AuthenticationContext                │
└────────────────┬────────────────────────┘
                 │
┌────────────────▼────────────────────────┐
│        MySQL Database (8.0)             │
│  - Divisions & Div_Zones                │
│  - Inventories & Records                │
│  - Audit_Admins & Roles                 │
└─────────────────────────────────────────┘
```

### Entity Relationship Diagram

```
┌──────────────────┐
│   Divisions      │ (Departments)
│──────────────────│
│ ID (PK)          │
│ Div_Code         │
│ Desc             │
│ CreatedDate      │
│ UpdatedDate      │
└────────┬─────────┘
         │ 1:N
         │
┌────────▼──────────┐
│   Div_Zones       │ (Sub-Departments)
│──────────────────│
│ ID (PK)          │
│ Zone_Code        │
│ Desc             │
│ DivID (FK)       │
│ CreatedDate      │
│ UpdatedDate      │
└────────┬─────────┘
         │ 1:N
         │
         │
┌────────────────────────┐
│   Inventories          │
│────────────────────────│
│ ID (PK)                │
│ desc                   │
│ CreatedDate            │
│ UpdatedDate            │
└────────┬───────────────┘
         │ 1:N
         │
┌────────▼─────────────────────────────┐
│   Inventory_Records                  │
│────────────────────────────────────│
│ ID (PK)                             │
│ ZoneID (FK to Div_Zones)             │
│ Status (0=NOT READY, 1=READY, 2=DONE)│
│ INVID (FK to Inventories)            │
│ CreatedDate                          │
│ UpdatedDate                          │
└──────────────────────────────────────┘


┌──────────────────┐
│   Roles          │
│──────────────────│
│ ID (PK)          │
│ Code             │
│ Desc             │
│ CreatedDate      │
│ UpdatedDate      │
└────────┬─────────┘
         │ 1:N
         │
┌────────▼──────────────────┐
│   Audit_Admins            │
│──────────────────────────│
│ Username (PK)            │
│ Name                     │
│ RoleID (FK to Roles)     │
│ CreatedDate              │
│ UpdatedDate              │
└──────────────────────────┘
```

---

## Database Models

### Model Files Location
All models are in `Audit_Tracker-DOTNET/Models/DB_Objects/`

### Core Inventory Models

#### 1. **Divisions.cs** - Department Level Organization
```csharp
public class Divisions
{
    [Key] public int ID { get; set; }
    [Required] public string Div_Code { get; set; }        // Department code (converted to uppercase)
    [Required] public string Desc { get; set; }            // Department description
    [Required] public ICollection<Div_Zones> Div_Items { get; set; } // Child zones
}
```
**Purpose:** Represents departments/divisions in the organization  
**Example:** SALES, MARKETING, OPERATIONS

---

#### 2. **Div_Zones.cs** - Sub-Department Level Organization
```csharp
public class Div_Zones
{
    [Key] public int ID { get; set; }
    [Required] public string Zone_Code { get; set; }       // Zone code (converted to uppercase)
    [Required] public string Desc { get; set; }            // Zone description
    public int DivID { get; set; }                         // Foreign key to Division
    public Divisions? Div { get; set; }                    // Navigation property
}
```
**Purpose:** Represents sub-departments/zones within divisions  
**Example:** SALES-NORTH, SALES-SOUTH, MARKETING-DIGITAL

---

#### 3. **Inventories.cs** - Audit Batch Container
```csharp
public class Inventories
{
    [Key] public string ID { get; set; }                   // Unique ID (generated)
    [Required] public string desc { get; set; }            // Audit description/title
    [Required] public ICollection<Inventory_Records> Records { get; set; } // Child records
}
```
**Purpose:** Represents a batch of inventory audit records  
**Example:** "Q1 2026 Audit", "Year-End Inventory 2025"

---

#### 4. **Inventory_Records.cs** - Individual Audit Record
```csharp
public class Inventory_Records
{
    [Key] public string ID { get; set; }                   // Unique record ID (generated)
    [Required] public int ZoneID { get; set; }             // Zone being audited (FK)
    [Required] public int Status { get; set; }             // 0=NOT READY, 1=READY, 2=COMPLETED
    public string INVID { get; set; }                      // Parent inventory ID (FK)
    public Inventories INV { get; set; }                   // Navigation property
}
```
**Status Codes:**
- `0` = NOT READY - Initial status, audit not started
- `1` = READY FOR TAG OFFICE - Audit data entered, pending transfer
- `2` = COMPLETED - Audit finalized and transferred

---

### Authentication Models

#### 5. **Audit_Admins.cs** - User Account
```csharp
public class Audit_Admins
{
    [Required][Key] public string Username { get; set; }   // Domain username (PK)
    [Required] public string Name { get; set; }            // Full name
    [Required] public int RoleID { get; set; }             // Role assignment (FK)
    public Roles Role { get; set; }                        // Navigation property
}
```
**Purpose:** Represents application users  
**Username Format:** Domain-stripped (e.g., "jsmith" instead of "AMU\jsmith")

---

#### 6. **Roles.cs** - Permission Level
```csharp
public class Roles
{
    [Key] public int ID { get; set; }
    [Required] public string Code { get; set; }            // Role identifier (Master, Auditor)
    [Required] public string Desc { get; set; }            // Role description
}
```
**Role Types:**
- **Master** - Full system access, can manage all entities and users
- **Auditor** - Can view and update inventory records

---

### System Models

#### 7. **Record_Pair.cs** - Data Transfer Object
```csharp
public class Record_Pair
{
    public Inventory_Records Record { get; set; }          // Inventory record
    public Div_Zones Zone { get; set; }                    // Associated zone
}
```
**Purpose:** Temporary pairing of records with their zones for processing

---

### Audit Trail Columns
All entities automatically include:
- `CreatedDate` (DateTime) - Set on record creation
- `UpdatedDate` (DateTime) - Updated on every modification

This is implemented in both DbContext's `OnModelCreating()` and `SaveChanges()` methods.

---

## Services & Business Logic

### Service Files Location
`Audit_Tracker-DOTNET/Services/`

### 1. **DB_Services.cs** - Core Data Operations
File: `Services/DB_Services/DB_Services.cs`

#### Constructor
```csharp
public DB_Services(IDbContextFactory<InventoryDbContext> dbcontext, ILogger<DB_Services> logger)
```

#### A. Division Management Methods

**GetAllDivisions()**
```csharp
public async Task<List<Divisions>> GetAllDivisions()
```
- **Purpose:** Retrieve all divisions
- **Returns:** List of all Division objects
- **Usage:** Populate division lists in dropdowns/tables

**GetSingleDivision(int id)**
```csharp
public async Task<Divisions> GetSingleDivision(int id)
```
- **Purpose:** Get a specific division by ID
- **Parameters:** id - Division ID
- **Returns:** Single Division object or null

**DivisionCreation(Divisions new_division)**
```csharp
public async Task<bool> DivisionCreation(Divisions new_division)
```
- **Purpose:** Create new division
- **Parameters:** new_division - Division object with Div_Code and Desc
- **Returns:** true if successful, false if exception occurs
- **Processing:** Converts Div_Code to uppercase

**UpdateDivision(Divisions new_Division)**
```csharp
public async Task UpdateDivision(Divisions new_Division)
```
- **Purpose:** Update existing division
- **Parameters:** new_Division - Updated Division object
- **Updates:** Div_Code and Desc fields only

**DivisionDelete(Divisions current_division)**
```csharp
public async Task<bool> DivisionDelete(Divisions current_division)
```
- **Purpose:** Delete a division
- **Parameters:** current_division - Division to delete
- **Returns:** true if successful
- **Warning:** May cascade delete associated zones

---

#### B. Zone Management Methods

**GetAllZones()**
```csharp
public async Task<List<Div_Zones>> GetAllZones()
```
- **Purpose:** Retrieve all zones across all divisions
- **Returns:** Complete list of Div_Zones

**GetZonesByDivision(int id)**
```csharp
public async Task<List<Div_Zones>> GetZonesByDivision(int id)
```
- **Purpose:** Get zones for a specific division
- **Parameters:** id - Division ID to filter
- **Returns:** List of zones in that division

**GetSingleZone(int key)**
```csharp
public async Task<Div_Zones> GetSingleZone(int key)
```
- **Purpose:** Get a specific zone by ID
- **Parameters:** key - Zone ID
- **Returns:** Single Div_Zones object

**CreateZone(Div_Zones new_zone)**
```csharp
public async Task<bool> CreateZone(Div_Zones new_zone)
```
- **Purpose:** Create new zone
- **Parameters:** new_zone - Zone object (must specify DivID)
- **Returns:** true if successful
- **Processing:** Converts Zone_Code to uppercase

**DeleteZone(Div_Zones zone)**
```csharp
public async Task<bool> DeleteZone(Div_Zones zone)
```
- **Purpose:** Delete a zone
- **Parameters:** zone - Zone to delete
- **Returns:** true if successful

---

#### C. Inventory Management Methods

**GetAllInventories()**
```csharp
public async Task<List<Inventories>> GetAllInventories()
```
- **Purpose:** Retrieve all inventory audits
- **Returns:** Complete list of Inventories

**GetSingleInventory(string id)**
```csharp
public async Task<Inventories> GetSingleInventory(string id)
```
- **Purpose:** Get specific inventory with all records
- **Parameters:** id - Inventory ID
- **Returns:** Single Inventories object with Records collection

**AddInventory(Inventories new_inventory)**
```csharp
public async Task<bool> AddInventory(Inventories new_inventory)
```
- **Purpose:** Create inventory and its records
- **Parameters:** new_inventory - Inventory with pre-populated Records collection
- **Returns:** true if successful
- **Process:** 
  1. Adds inventory to database
  2. Iterates through Records collection
  3. Calls AddRecord() for each record
  4. Saves all changes

**DeleteInventory(Inventories inventory)**
```csharp
public async Task<bool> DeleteInventory(Inventories inventory)
```
- **Purpose:** Delete inventory and cascade delete records
- **Parameters:** inventory - Inventory to delete
- **Returns:** true if successful

---

#### D. Inventory Record Management Methods

**GetAllRecords()**
```csharp
public async Task<List<Inventory_Records>> GetAllRecords()
```
- **Purpose:** Get all audit records across all inventories
- **Returns:** Complete list of Inventory_Records

**GetRecordsByInventory(string ID)**
```csharp
public async Task<List<Inventory_Records>> GetRecordsByInventory(string ID)
```
- **Purpose:** Get master record list for an inventory (all zones)
- **Parameters:** ID - Inventory ID
- **Returns:** List of records for that inventory ordered by zone

**GetRecordsByInventoryPerDivision(List<Inventory_Records> record_master_list, int division_id)**
```csharp
public async Task<List<Inventory_Records>> GetRecordsByInventoryPerDivision(
    List<Inventory_Records> record_master_list, 
    int division_id)
```
- **Purpose:** Filter records by division (advanced query)
- **Parameters:** 
  - record_master_list - Pre-fetched master list
  - division_id - Division to filter by
- **Process:**
  1. Fetches all zones
  2. Creates Record_Pair objects (Record + Zone)
  3. Filters by division_id
  4. Returns only records for that division
- **Use Case:** Division-level progress tracking

**GetSingleRecord(string id)**
```csharp
public async Task<Inventory_Records> GetSingleRecord(string id)
```
- **Purpose:** Get specific audit record
- **Parameters:** id - Record ID
- **Returns:** Single Inventory_Records object

**AddRecord(Inventory_Records record)**
```csharp
public async Task<bool> AddRecord(Inventory_Records record)
```
- **Purpose:** Create new audit record
- **Parameters:** record - Record with ZoneID and Status set
- **Returns:** true if successful

**UpdateRecord(Inventory_Records new_record)**
```csharp
public async Task<bool> UpdateRecord(Inventory_Records new_record)
```
- **Purpose:** Update record status
- **Parameters:** new_record - Record with new Status value
- **Updates:** Only Status field
- **Use Case:** Mark record as READY for TAG OFFICE or COMPLETED

---

#### E. Utility Methods

**IDGenerator()**
```csharp
public string IDGenerator()
```
- **Purpose:** Generate unique IDs for inventories and records
- **Returns:** Unique string identifier
- **Format:** `INV{random_double}{random_int}{unix_timestamp}{microsecond}{day}{month}{year}`
- **Distribution:** Uses random + timestamp components to ensure uniqueness

---

### 2. **Admin_Services.cs** - User Management
File: `Services/DB_Services/Admin_Services.cs`

#### Constructor
```csharp
public Admin_Services(IDbContextFactory<AuthenticationContext> dbcontext, ILogger<AuthenticationContext> logger)
```

#### Methods

**GetAllUsers()**
```csharp
public async Task<List<Audit_Admins>> GetAllUsers()
```
- **Purpose:** Retrieve all system users
- **Returns:** List of all Audit_Admins

**GetAdmin(string username)**
```csharp
public async Task<Audit_Admins> GetAdmin(string username)
```
- **Purpose:** Get specific user by domain username
- **Parameters:** username - Domain account name (e.g., "jsmith")
- **Returns:** Single Audit_Admins object or null

**GetAllRoles()**
```csharp
public async Task<List<Roles>> GetAllRoles()
```
- **Purpose:** Retrieve all role definitions
- **Returns:** List of all Roles

**GetRole(int ID)**
```csharp
public async Task<Roles> GetRole(int ID)
```
- **Purpose:** Get specific role definition
- **Parameters:** ID - Role ID
- **Returns:** Single Roles object

**UpdateUser(Audit_Admins new_user)**
```csharp
public async Task UpdateUser(Audit_Admins new_user)
```
- **Purpose:** Update user profile
- **Parameters:** new_user - User object with updated Name and RoleID
- **Updates:** Name and RoleID fields
- **Process:** Fetches existing user, updates properties, saves

**DeleteUser(string username)**
```csharp
public async Task DeleteUser(string username)
```
- **Purpose:** Remove user from system
- **Parameters:** username - User to delete
- **Process:** Fetches user by username and removes from database

---

### 3. **CustomAuthenticationStateProvider.cs** - Authentication Handler
File: `Services/Authentication/CustomAuthenticationStateProvider.cs`

#### Purpose
Extends `AuthenticationStateProvider` to manage Blazor authentication state with Windows AD integration

#### Methods

**GetAuthenticationStateAsync()**
```csharp
public override async Task<AuthenticationState> GetAuthenticationStateAsync()
```
- **Purpose:** Get current authenticated user and inject role claims
- **Returns:** AuthenticationState with user claims
- **Process:**
  1. Retrieves HttpContext user (Windows authenticated)
  2. Validates user against database
  3. Adds role claims based on database roles
  4. Returns updated claims principal

**ValidateAudit(string username)**
```csharp
public async Task<bool> ValidateAudit(string username)
```
- **Purpose:** Check if user has Auditor role
- **Parameters:** username - Full domain username (e.g., "AMU\jsmith")
- **Process:**
  1. Strips domain prefix (removes "AMU\\")
  2. Queries database for user
  3. Checks role code against SD.Auditor ("Auditor")
- **Returns:** true if user is auditor

**ValidateMaster(string username)**
```csharp
public async Task<bool> ValidateMaster(string username)
```
- **Purpose:** Check if user has Master role
- **Parameters:** username - Full domain username
- **Process:** Same as ValidateAudit, checks for SD.Master ("Master")
- **Returns:** true if user is master

---

### 4. **Authorize_Policy.cs** - Authorization Policies
File: `Services/Authentication/Authorize_Policy.cs`

#### Method

**AddCustomPolicies(AuthorizationOptions options)**
```csharp
public static void AddCustomPolicies(AuthorizationOptions options)
```
- **Purpose:** Define custom authorization policies for application
- **Called In:** Program.cs during startup
- **Policies Defined:**

| Policy | Requirement | Usage |
|--------|-------------|-------|
| `SD.Master` | User has "Master" claim in ClaimTypes.Role | `/master` page, admin functions |
| `SD.Auditor` | User has "Auditor" claim in ClaimTypes.Role | Record viewing/editing |

---

## Authentication & Authorization

### Authentication Flow

```
1. Windows AD Login (Active Directory)
   ↓
2. IIS/AspNetCore Captures Identity
   ↓
3. HttpContext.User populated with Windows identity
   ↓
4. Routes.razor Wraps in CascadingAuthenticationState
   ↓
5. CustomAuthenticationStateProvider.GetAuthenticationStateAsync() invoked
   ↓
6. Strips domain prefix (AMU\) and queries Audit_Admins table
   ↓
7. Adds role claims (Master and/or Auditor)
   ↓
8. AuthorizeView/AuthorizeRouteView uses claims for page access
   ↓
9. User either sees content or Unauthorized component
```

### Authorization Levels

#### Master Role
- **Access:** Full system administration
- **Permissions:**
  - View `/master` page
  - CRUD all Divisions
  - CRUD all Zones
  - CRUD all Inventories
  - CRUD all Users
  - CRUD all Roles
  - Edit any Record
  - Access all pages

#### Auditor Role
- **Access:** Standard audit operations
- **Permissions:**
  - View Home page
  - View Inventory list
  - View Records for assigned inventories
  - Update Record status
  - View own profile
  - Cannot access `/master` page

#### Unauthenticated/Unauthorized
- **Access:** Denied
- **Display:** Unauthorized.razor component
- **Redirect:** Home page or disables navigation links

### Domain Configuration (SD.cs)

```csharp
public const string UserDomain = "AMU\\";                          // Domain prefix
public const string Master = "Master";                             // Master role code
public const string Auditor = "Auditor";                           // Auditor role code

public readonly List<string> AllowedSIDs = new(){
    "S-1-5-21-4127812034-820336945-2256232113-513" // Domain Users SID
};
```

---

## Pages & Routes

### Route Structure

#### 1. **Home.razor** - Landing Page
- **Route:** `/`
- **Access:** Anyone (with authentication)
- **Components:** Inventory_Main
- **Purpose:** Display all inventories for selection

---

#### 2. **Inventory_Single.razor** - Inventory Details
- **Route:** `/inventory/{ID}`
- **Parameters:** ID - Inventory ID
- **Access:** Authenticated users
- **Features:**
  - Shows inventory description and audit date
  - Displays progress bar (% complete)
  - Grid of divisions with cards
  - Real-time refresh every 10 seconds
  - Breadcrumb navigation
- **Built-In Method:** `InvokeAsync(() => StateHasChanged())` for UI updates

---

#### 3. **Records.razor** - Division Records
- **Route:** `/records/{Current_inv}/{div_id}`
- **Parameters:** 
  - Current_inv - Inventory ID
  - div_id - Division ID
- **Access:** Auditors and Masters
- **Features:**
  - Displays all zones/records in division
  - Shows current status for each record
  - Allows status updates
  - Different UI for Masters vs Auditors

---

#### 4. **Division.razor** - Division Zone Table
- **Route:** `/division` or `/division/{division_code}`
- **Parameters:** division_code - Division code (optional)
- **Components:** ZoneTable
- **Purpose:** Quick view of zones in a division

---

#### 5. **MasterPage.razor** - Administration Panel
- **Route:** `/master`
- **Access:** Master role only
- **Requires:** AuthorizeView with Policy="Master"
- **Content:** Tabbed interface
- **Tabs:**
  - **Inventories Tab:** Inventory_Master component
    - Create/Read/Update/Delete inventories
    - Manage audit batch records
  - **Users Tab:** User_Master component
    - Create/Read/Update/Delete users
    - Assign roles
  - **Divisions Tab:** Division__Master component
    - Manage departments
    - Create/update division codes
  - **Zones Tab:** Zone_Master component
    - Manage sub-departments
    - Assign zones to divisions
- **Unauthorized:** Shows Unauthorized.razor component

---

#### 6. **ClaimTest.razor** - Claims Testing (Debug)
- **Route:** `/claims`
- **Access:** Authorized users only
- **Purpose:** Display user's security claims for debugging
- **Display:** Table with Type and Value columns
- **Use Case:** Verify role claims are being added correctly

---

#### 7. **DB_Test_Page.razor** - Database Testing (Debug)
- **Route:** `/Test`
- **Purpose:** Test database CRUD operations
- **Features:**
  - List all inventories
  - Delete button for each inventory
  - Add new inventory button
  - Generate all zone records on creation
- **Use Case:** Development/testing only

---

#### 8. **Error.razor** - Error Page
- **Route:** `/Error`
- **Purpose:** Display unhandled exceptions
- **Shows:** Error message, Request ID (in dev mode)

---

### Page Component Hierarchy

```
Home.razor (/)
├── Inventory_Main
│   └── Inventory_Minor (per inventory)
│
Inventory_Single.razor (/inventory/{ID})
├── DivisionCard (per division)
│   └── Links to Records.razor
│
Records.razor (/records/{inv}/{div})
├── RecordsCard (per record/zone)
│   └── Status display & update controls
│
Division.razor (/division/{code})
├── ZoneTable
│   └── MudDataGrid display
│
MasterPage.razor (/master) [Protected]
├── Inventory_Master
├── User_Master
├── Division__Master
├── Zone_Master
└── Unauthorized (if not Master role)
```

---

## Components

### Layout Components

#### **MainLayout.razor**
Location: `Components/Layout/MainLayout.razor`
- **Features:**
  - MudLayout with AppBar, Drawer, MainContent structure
  - Top AppBar with hamburger menu and title "Inventory Audits"
  - Collapsible navigation drawer
  - Clock component displaying real-time
  - Footer component

#### **NavMenu.razor**
Location: `Components/Layout/NavMenu.razor`
- **Features:**
  - Displays authenticated username
  - Home link (visible to all authenticated)
  - Master Page link (conditionally visible only to Master role)
  - Uses MudNavMenu for styling
  - Responsive drawer navigation

#### **Clock.razor**
Location: `Components/Layout/Clock.razor`
- **Features:**
  - Real-time clock display
  - Updates every 1000ms using System.Timers.Timer
  - Format: Full date and time (e.g., "Wednesday, March 5, 2026 12:34:56 PM")
  - Calls InvokeAsync and StateHasChanged for UI updates

#### **Footer.razor**
Location: `Components/Layout/Footer.razor`
- **Purpose:** Footer section (currently mostly empty)
- **Styling:** Dark background with padding

---

### Utility Components

#### **ZoneTable.razor**
Location: `Components/Utils/ZoneTable.razor`
- **Purpose:** Display zones for a division in table format
- **UI:** MudDataGrid
- **Parameters:** Div_Code (division code filter)
- **Features:** Refresh button for zone updates
- **Status:** Partially implemented (some code commented out)

#### **Unauthorized.razor**
Location: `Components/Utils/Unauthorized.razor`
- **Purpose:** Generic unauthorized message
- **Display:** Simple "Unauthorized" heading
- **Used In:** MasterPage.razor when user lacks required role

---

### Page Components (Referenced in Routes)

#### **Inventory_Main**
- **Purpose:** Display master list of inventories
- **Features:**
  - Fetches all inventories via DB_Services
  - Groups by year
  - Shows loading progress bar
  - Individual Inventory_Minor components

#### **Inventory_Minor**
- **Purpose:** Individual inventory card
- **Features:**
  - Clickable button
  - Navigates to /inventory/{ID}

#### **Inventory_Single** (implicit)
- **Purpose:** Inventory detail view
- **Features:**
  - Progress bar calculation
  - Division cards
  - Real-time refresh timer (10 second interval)

#### **DivisionCard**
- **Purpose:** Card for each division in inventory
- **Features:**
  - Division name and description
  - Click to navigate to Records.razor

#### **RecordsCard**
- **Purpose:** Individual record display in division
- **Features:**
  - Zone information
  - Status indicator
  - Status update button/dropdown

---

## Configuration

### appsettings.json
**File:** `Audit_Tracker-DOTNET/appsettings.json`
```json
{}
```
- **Note:** Empty by design; all config in Development/Production variants

---

### appsettings.Development.json
**File:** `Audit_Tracker-DOTNET/appsettings.Development.json`
```json
{
  "ConnectionStrings": {
    "MySQLConection": "server=localhost;Port=3306;uid=root;pwd=password;database=aap_inventory_db_qa"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```
- **Connection:** Local MySQL development server
- **Logging:** Standard ASP.NET Core logging levels

---

### appsettings.Production.json
**File:** `Audit_Tracker-DOTNET/appsettings.Production.json`
```json
{
  "ConnectionStrings": {
    "MySQLConection": "server=hmaapp350.am.mds.honda.com;Port=3306;uid=AAP_INV;pwd=P@SS4INV;database=aap_inventory_db_qa"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```
- **Connection:** Honda cloud MySQL server (hmaapp350)
- **Credentials:** Service account AAP_INV
- **Database:** aap_inventory_db_qa

---

### launchSettings.json
**File:** `Audit_Tracker-DOTNET/Properties/launchSettings.json`

**IIS Express Profile:**
```json
{
  "iisSettings": {
    "windowsAuthentication": true,
    "anonymousAuthentication": false,
    "iisExpress": {
      "applicationUrl": "http://localhost:53510",
      "sslPort": 44344
    }
  }
}
```

**HTTPS Profile (Standalone):**
```json
{
  "commandName": "Project",
  "applicationUrl": "https://localhost:7291;http://localhost:5245",
  "environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development"
  }
}
```

**HTTP Profile (Standalone):**
```json
{
  "commandName": "Project",
  "applicationUrl": "http://localhost:5245",
  "environmentVariables": {
    "ASPNETCORE_ENVIRONMENT": "Development"
  }
}
```

---

### Project File (Audit_Tracker-DOTNET_Blazor.csproj)

**Target Framework:**
```xml
<TargetFramework>net8.0</TargetFramework>
<Nullable>enable</Nullable>
<ImplicitUsings>enable</ImplicitUsings>
```

**NuGet Package Dependencies:**
| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.EntityFrameworkCore | 8.0.13 | ORM for database access |
| Microsoft.EntityFrameworkCore.Design | 8.0.13 | EF Core tools for migrations |
| Microsoft.EntityFrameworkCore.Tools | 8.0.13 | Package Manager Console tools |
| MudBlazor | 8.2.0 | Material Design UI components |
| MySql.EntityFrameworkCore | 8.0.11 | MySQL database provider for EF Core |

**Excluded Folders:**
```xml
<Folder Include="Components\Pages\Division Components\Pages\" />
<Folder Include="Components\Pages\Home Components\Pages\" />
<Folder Include="Components\Pages\Record Components\Pages\" />
```

---

### SD.cs - Static Configuration
**File:** `Audit_Tracker-DOTNET/SD.cs`

```csharp
namespace Main_SD
{
    public class SD
    {
        // Status definitions
        public readonly Record_stat[] statuses = {
            new() { code = 0, desc = "NOT READY" },
            new() { code = 1, desc = "READY FOR TAG OFFICE" },
            new() { code = 2, desc = "COMPLETED" }
        };

        public readonly string[] labels = 
            { "NOT READY", "READY FOR TAG OFFICE", "COMPLETED" };

        // Active Directory configuration
        public readonly List<string> AllowedSIDs = new(){
            "S-1-5-21-4127812034-820336945-2256232113-513" // Domain Users
        };

        public const string UserDomain = "AMU\\";
        public const string Master = "Master";
        public const string Auditor = "Auditor";
    }

    public class Record_stat
    {
        public int code { get; set; }
        public string desc { get; set; }
    }
}
```

---

## Running the Application

### Option 1: Visual Studio (Recommended for Development)

1. **Open Solution:**
   ```bash
   start Audit_Tracker-DOTNET.sln
   ```

2. **Restore Packages:**
   - Packages → Manage NuGet Packages
   - Or: `dotnet restore`

3. **Run Database Migrations:**
   ```bash
   # Package Manager Console
   Update-Database -Context InventoryDbContext
   Update-Database -Context AuthenticationContext
   ```

4. **Start Debugging:**
   - Press `F5` or Click "Run"
   - Select IIS Express or HTTPS profile
   - Browser opens to `https://localhost:7291`

5. **Login:**
   - Uses Windows Authentication
   - Automatic login with your domain account (AMU\username)

---

### Option 2: IIS Express (Command Line)

```bash
cd Audit_Tracker-DOTNET
dotnet run --launch-profile "IIS Express"
```
- Launches at `http://localhost:53510`
- Remember to enable Windows Authentication

---

### Option 3: Standalone .NET CLI

```bash
cd Audit_Tracker-DOTNET

# Development (HTTP)
dotnet run --configuration Debug --launch-profile "http"
# Launches at http://localhost:5245

# HTTPS
dotnet run --configuration Debug --launch-profile "https"
# Launches at https://localhost:7291;http://localhost:5245
```

---

### Expected Startup Sequence

1. Application starts and builds Blazor components
2. Initializes MudBlazor services
3. Connects to MySQL database (validates connection string)
4. Sets up dependency injection for DB_Services, Admin_Services
5. Configures authentication (Windows) and authorization (custom policies)
6. Routes.razor mounts and wraps page in CascadingAuthenticationState
7. Browser redirects to home page
8. CustomAuthenticationStateProvider queries user's roles
9. NavMenu displays based on user's authenticated state
10. Home page loads with inventory list

---

## Deployment

### Target Environments

#### 1. **Development**
- **Host:** Local machine or dev server
- **Database:** appsettings.Development.json (localhost MySQL)
- **Auth:** Windows Auth to local domain or test account
- **Profile:** IIS Express or standalone `dotnet run`

#### 2. **QA/Testing**
- **Host:** Internal test server
- **Database:** appsettings.json (QA instance)
- **Auth:** AMU domain Windows Authentication
- **Build:** `dotnet publish -c Release`

#### 3. **Production**
- **Host:** Honda internal IIS server (hmaapp350)
- **Database:** appsettings.Production.json (hmaapp350.am.mds.honda.com)
- **Auth:** AMU domain Windows Authentication
- **Credentials:** AAP_INV service account

---

### Pre-Deployment Checklist

- [ ] Review connection strings in all appsettings files
- [ ] Verify MySQL server is reachable from target environment
- [ ] Confirm Windows domain authentication is configured in IIS
- [ ] Update allowed SIDs in SD.cs if using different domain group
- [ ] Create database and run migrations:
  ```bash
  dotnet ef database update --context InventoryDbContext
  dotnet ef database update --context AuthenticationContext
  ```
- [ ] Seed admin users in Audit_Admins table
- [ ] Test role-based access with sample Master and Auditor accounts
- [ ] Review MudBlazor CSS/JS assets are in wwwroot
- [ ] Validate all component references compile without errors

---

### Publishing Steps

1. **Build Release Version:**
   ```bash
   cd Audit_Tracker-DOTNET
   dotnet publish -c Release -o ./publish
   ```

2. **Copy to Server:**
   ```bash
   # Copy publish folder to IIS server
   xcopy publish \\server\iis\audit-tracker /E /I /Y
   ```

3. **IIS Configuration:**
   - Create new website or app pool
   - Set physical path to published folder
   - Enable Windows Authentication
   - Disable Anonymous Authentication
   - Set app pool identity to appropriate service account

4. **Database Setup (On Target Server):**
   ```bash
   # Run migrations on target
   dotnet ef database update --context InventoryDbContext
   dotnet ef database update --context AuthenticationContext
   ```

5. **Test:**
   - Browse to application URL
   - Verify domain user auto-login works
   - Test role-based access
   - Verify reports and data access

---

### Runtime Requirements on Deployment Server

- **.NET 8.0 Runtime** (or SDK if running from source)
- **IIS 10.0+** with:
  - Application Request Routing (ARR)
  - Windows Authentication module
  - URL Rewrite
  - .NET Core Hosting Bundle for IIS
- **MySQL Connector** (on database server, not required on web server)
- **Windows Domain Membership** (for authentication)

---

## Troubleshooting

### Connection String Errors

**Error:** `No connection string in config!`
- **Cause:** appsettings has empty ConnectionStrings
- **Fix:** Add MySQLConection to appsettings.Development.json
- **Verify:** Test connection with MySQL Workbench or command line

---

### Authentication Issues

**Error:** User can't login / Page shows "Unauthorized"
- **Cause:** Windows Authentication not enabled
- **Solutions:**
  1. Check IIS Express settings: Project Properties → Debug → Windows Authentication enabled
  2. Verify user account exists in database: `SELECT * FROM Audit_Admins WHERE Username='jsmith'`
  3. Verify role assignment: `SELECT * FROM Roles WHERE ID=<RoleID>`

**Error:** Role-based features unavailable (Master link disabled)
- **Cause:** User's role not loaded correctly
- **Debug:** Navigate to `/claims` page to see actual claims
- **Fix:** Verify CustomAuthenticationStateProvider ValidateMaster() returns true

---

### Database Migration Issues

**Error:** `There is already an object named 'Divisions' in the database`
- **Cause:** Migration already applied
- **Solution:** 
  ```bash
  dotnet ef migrations script --context InventoryDbContext
  # Review script; apply manually if needed
  ```

**Error:** `The entity type 'Divisions' has two properties named 'CreatedDate'`
- **Cause:** Duplicate shadow properties in OnModelCreating
- **Fix:** Context already adds CreatedDate/UpdatedDate automatically; don't add again

---

### Blazor Component Issues

**Error:** Component not rendering / "No Record to show" everywhere
- **Cause:** Data not loading from DB_Services
- **Debug:** 
  1. Add Console.WriteLine() in component's OnInitializedAsync
  2. Check browser's F12 Developer Tools → Console for client-side errors
  3. Check server logs for exceptions

**Error:** MudBlazor styles not applied (buttons/grids look plain)
- **Cause:** Missing CSS references in App.razor
- **Fix:** Verify App.razor has:
  ```html
  <link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />
  <script src="_content/MudBlazor/MudBlazor.min.js"></script>
  ```

---

### Performance Issues

**Slow Page Loads:**
- Check for synchronous database calls (use async/await)
- Verify indexes exist on frequently queried columns (ID, DivID, INVID)
- Consider pagination for large inventory lists

**Database Connection Pool Exhaustion:**
- Ensure IDbContextFactory is used (not singleton DbContext)
- Check for unclosed contexts: verify `using` statements or `Dispose()` 

---

## Development Guidelines

### Code Style
- Use C# naming conventions (PascalCase for classes, camelCase for fields)
- Use async/await for all database operations
- Handle exceptions and log to ILogger

### Database Changes
1. Create migration: `dotnet ef migrations add DescriptiveName --context ContextName`
2. Review generated migration file in Migrations folder
3. Test: `dotnet ef database update`
4. Commit migration files to source control

### Adding New Features
1. **Add model** in `Models/DB_Objects/`
2. **Add DbSet** to appropriate DbContext
3. **Create migration** and update database
4. **Add CRUD methods** to appropriate Service class
5. **Inject service** in Blazor component via @inject
6. **Create/modify page/component** in Components/Pages/

### Testing
- Use `/Test` and `/claims` pages for functionality validation
- Test both Master and Auditor roles
- Verify database cascade deletes work as expected
- Test with varying numbers of inventories/records

---

## Summary

This Audit Tracker application provides a complete inventory audit management platform built on .NET 8 with Blazor interactive components. It integrates Windows domain authentication, role-based authorization, and a comprehensive MySQL database schema for tracking department-level inventory audits. The modular service architecture and component-based UI make it maintainable and extensible for future feature additions.

For questions or issues, check the troubleshooting section or review the specific service/model documentation above.
