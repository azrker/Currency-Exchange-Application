# 💱 Currency Exchange Application

|||
|-|-|
|**Course**|Network Application Development|
|**Project Title**|Currency Exchange — WCF Service with WPF Desktop Client|
|**Author**|*(Azar Karimov)*|
|**Student ID**|*(65643)*|
|**Framework**|.NET Framework 4.7.2|
|**Service Type**|WCF (SOAP / BasicHttpBinding)|
|**Client Type**|WPF Desktop Application|

\---

## 📋 Table of Contents

1. [Project Overview](#-project-overview)
2. [Architecture](#-architecture)
3. [Project Structure](#-project-structure)
4. [Features](#-features)
5. [Technologies Used](#-technologies-used)
6. [Prerequisites](#-prerequisites)
7. [How to Run](#-how-to-run)
8. [How to Use the Application](#-how-to-use-the-application)
9. [External API](#-external-api)




## 📌 Project Overview

**Currency Exchange** is a network desktop application that simulates a currency exchange office. It is built using a **WCF (Windows Communication Foundation)** backend service and a **WPF (Windows Presentation Foundation)** desktop client.

The service connects in real time to the **Polish National Bank (NBP) public API** to fetch up to date exchange rates for all major currencies. Users can register accounts, deposit PLN funds, buy and sell foreign currencies, and review their full transaction history and historical rate charts all through a modern dark themed desktop interface.

This project demonstrates a full client server architecture using SOAP-based WCF communication over HTTP, following the Network Application Development course requirements.

\---

## 🏗 Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                        CLIENT SIDE                          │
│                                                             │
│   ┌──────────────────────┐   ┌────────────────────────┐    │
│   │   WPF Desktop Client  │   │  Console Test Client   │    │
│   │  (CurrencyExchange.   │   │  (CurrencyExchange.    │    │
│   │    WpfClient)         │   │    TestClient)         │    │
│   └──────────┬───────────┘   └──────────┬─────────────┘    │
│              │  BasicHttpBinding (SOAP)  │                  │
└──────────────┼───────────────────────────┼──────────────────┘
               │                           │
               ▼                           ▼
┌─────────────────────────────────────────────────────────────┐
│                     SERVICE SIDE                            │
│                                                             │
│   ┌─────────────────────────────────────────────────────┐  │
│   │         CurrencyExchange.Service (WCF)               │  │
│   │         Hosted on IIS Express (localhost)            │  │
│   │         http://localhost:{port}/CurrencyService.svc  │  │
│   │                                                      │  │
│   │  ICurrencyService (Contract)                         │  │
│   │  CurrencyService  (Implementation)                   │  │
│   └──────────────────────┬──────────────────────────────┘  │
│                          │ HTTP REST                        │
└──────────────────────────┼──────────────────────────────────┘
                           │
                           ▼
              ┌────────────────────────┐
              │   NBP Public API       │
              │   api.nbp.pl           │
              │   (Live exchange rates)│
              └────────────────────────┘
```

**Communication flow:**

1. The WCF Service is hosted locally via IIS Express (port is auto assigned by Visual Studio)
2. Clients connect using `BasicHttpBinding` (SOAP over HTTP)
3. The service fetches live exchange rate data from the NBP REST API
4. All user data (accounts, balances, transactions) is stored in memory during the service session

\---

## 📁 Project Structure

```
CurrencyExchange/
│
├── CurrencyExchange.Service/              # WCF Backend Service
│   ├── ICurrencyService.cs                # Service contract (interface + data contracts)
│   ├── CurrencyService.svc                # Service entry point declaration
│   ├── CurrencyService.svc.cs             # Full service implementation
│   ├── Web.config                         # Service configuration (IIS, WCF bindings)
│   ├── Web.Debug.config                   # Debug specific config transforms
│   ├── Web.Release.config                 # Release specific config transforms
│   ├── packages.config                    # NuGet dependencies (Newtonsoft.Json)
│   └── Properties/
│       └── AssemblyInfo.cs
│
├── CurrencyExchange.WpfClient/            # WPF Desktop Client
│   ├── MainWindow.xaml                    # Full UI layout (dark theme, panels)
│   ├── MainWindow.xaml.cs                 # All UI logic and service calls
│   ├── App.xaml                           # WPF application entry
│   ├── App.config                         # WCF endpoint configuration
│   ├── Connected Services/
│   │   └── CurrencyServiceRef/            # Auto generated WCF proxy
│   │       ├── Reference.cs               # Generated client proxy class
│   │       ├── CurrencyService.wsdl       # WSDL contract
│   │       └── Reference.svcmap           # Service map metadata
│   └── Properties/
│
├── CurrencyExchange.TestClient/           # Console Test Client
│   ├── Program.cs                         # Basic test: HelloWorld + USD/EUR rates
│   ├── App.config                         # WCF endpoint configuration
│   └── Connected Services/
│       └── CurrencyServiceRef/            # Auto generated WCF proxy
│
├── CurrencyExchange.slnx                  # Visual Studio solution file
└── README.md                              # This file
```

\---

## ✨ Features

### 🔐 User Authentication

* **Register** a new account with a username (min. 3 characters) and password (min. 4 characters)
* **Login** with credentials, with validation feedback
* **Password show/hide** toggle (👁 / 🙈) on the login screen
* **Sign out** with a confirmation dialog
* Session based user state maintained while the app is running

### 📈 Live Exchange Rates

* Fetches all currency rates from the **NBP Table A** in real time
* Displays currency **Code**, **Name**, and **Mid rate** (in PLN) in a sortable data grid
* **Refresh** button to update rates on demand
* Online/Offline status indicator in the top bar

### 💼 Account Management

* View current **PLN balance**
* **Top up** PLN balance by entering any positive amount
* Input validation with user friendly error messages

### 💰 Buy Currency

* Enter any **currency code** (e.g. `USD`, `EUR`, `GBP`) and an **amount**
* Service checks the live NBP rate and deducts the PLN cost from your balance
* Returns a confirmation with the exact rate used and PLN spent
* Validates for: missing input, invalid amount, insufficient PLN balance

### 💸 Sell Currency

* Enter a **currency code** and **amount** to sell back for PLN
* Service calculates earnings at the current live NBP rate
* Validates for: missing input, invalid amount, insufficient currency balance

### 📋 Transaction History

* Displays a full history table of all BUY and SELL operations
* Columns: **ID**, **Type**, **Currency**, **Amount**, **Rate**, **PLN Value**, **Date**

### 📊 Historical Rates

* Enter a **currency code**, **start date**, and **end date**
* Fetches historical mid rates from the NBP API for that date range
* Displays results in a table with **Date** and **Rate** columns

### 🖥️ Test Client (Console)

* A lightweight console project that connects to the service and prints:

  * Service health check (`HelloWorld`)
  * Live USD and EUR exchange rates

\---

## 🛠 Technologies Used

|Technology|Version|Purpose|
|-|-|-|
|C# / .NET Framework|4.7.2|Core language and runtime|
|WCF (Windows Communication Foundation)|Built-in|Backend service layer (SOAP/HTTP)|
|WPF (Windows Presentation Foundation)|Built-in|Desktop client UI|
|IIS Express|Built-in with VS|Service hosting during development|
|Newtonsoft.Json|13.0.4|Parsing JSON responses from NBP API|
|NBP Public REST API|—|Live \& historical PLN exchange rates|
|BasicHttpBinding|—|WCF transport binding (SOAP over HTTP)|
|XAML|—|UI layout and custom control templates|

\---

## ✅ Prerequisites

Before running this project, make sure you have the following installed:

* **Visual Studio 2022** (Community, Professional, or Enterprise)

  * Workload: **ASP.NET and web development** (for WCF Service hosting)
  * Workload: **.NET desktop development** (for WPF Client)
* **.NET Framework 4.7.2** (included with Visual Studio)
* **Internet connection** — required to fetch live rates from `api.nbp.pl`

> ℹ️ No database installation is required. All data is stored in memory.

\---

## ▶️ How to Run

### Step 1 — Open the Solution

Open `CurrencyExchange.slnx` in **Visual Studio 2022**.

You should see 3 projects in Solution Explorer:

* `CurrencyExchange.Service`
* `CurrencyExchange.WpfClient`
* `CurrencyExchange.TestClient`

\---

### Step 2 — Set Multiple Startup Projects

The service **must** start before the client. To configure this:

1. Right-click the **Solution** (top item in Solution Explorer) → **Properties**
2. Go to **Common Properties → Startup Project**
3. Select **"Multiple startup projects"**
4. Set the actions as follows:

|Project|Action|
|-|-|
|`CurrencyExchange.Service`|**Start**|
|`CurrencyExchange.WpfClient`|**Start**|
|`CurrencyExchange.TestClient`|**None**|

5. Click **OK**

\---

### Step 3 — Build the Solution

Press **Ctrl + Shift + B** to build all projects and restore NuGet packages.

\---

### Step 4 — Run the Application

Press **F5** (or click the green ▶️ **Start** button).

Two things will happen:

* **IIS Express** starts hosting the WCF Service at `http://localhost:{port}/CurrencyService.svc`
* The **WPF Client window** opens and connects to the service automatically

\---

## 🧭 How to Use the Application

### Register a new account

1. On the **Login screen**, enter a username (min. 3 chars) and password (min. 4 chars)
2. Click **Register**
3. You will see a green ✅ success message

### Log in

1. Enter your registered username and password
2. Click **Sign In**
3. You are taken to the **Live Rates** screen automatically

### Check live rates

* The **Rates** panel loads automatically after login
* Click **Refresh Rates** any time to fetch the latest NBP data

### Top up your balance

1. Go to **My Account** in the sidebar
2. Enter an amount in the Top Up field
3. Click **Top Up** — your PLN balance updates immediately

### Buy currency

1. Go to **Buy Currency** in the sidebar
2. Enter the currency code (e.g. `USD`, `EUR`, `CHF`, `GBP`)
3. Enter the amount you want to buy
4. Click **Buy** — the PLN cost is deducted from your balance

### Sell currency

1. Go to **Sell Currency** in the sidebar
2. Enter the currency code and amount to sell
3. Click **Sell** — you receive PLN at the current rate

### View transaction history

* Go to **Transactions** in the sidebar
* All your BUY and SELL operations are listed with full details

### View historical rates

1. Go to **History** in the sidebar
2. Enter a currency code and date range (format: `YYYY-MM-DD`)
3. Click **Get Rates** to see the historical mid rates from NBP

### Sign out

* Click **Sign Out** in the sidebar
* Confirm the dialog — you are returned to the login screen



## 🌐 External API

This project uses the **Narodowy Bank Polski (NBP) Web API** — a free, public REST API.

## 

