# Crypto Price Tracker – Full Stack Developer Challenge - Dwayne Love

## 📋 Objective

This challenge is designed to evaluate your skills as a full stack developer using .NET 6 (C#), Entity Framework Core, Razor Pages, and REST API integration.

You'll be working with a project that simulates tracking cryptocurrency prices using the CoinGecko API.

---

## 🧠 What You Need to Do

### ✅ 1. Fix the Service Logic

- Open `CryptoPriceService.cs` and fix the logic so it correctly:
  - Fetches coin data from CoinGecko for all existing coins  
    Documentation: https://docs.coingecko.com/v3.0.1/reference/introduction
  - Handles async operations properly
  - Prevents duplicate entries
  - Saves the results to the SQLite database

  ⚠️ If you encounter duplicate entries or errors from the CoinGecko API, decide how to handle them.  
  You're free to choose the best approach — just make sure you explain your reasoning in a short code comment.

  ⚠️ Some files may have outdated or incorrect `namespace` declarations. Please review and update them as needed to ensure consistency across the project.

### ✅ 2. Complete the Controller

- Open `CryptoController.cs` and:
  - Complete the `POST /api/crypto/update-prices` endpoint
  - Add a `GET /api/crypto/latest-prices` endpoint to return the most recent price per asset. All available cryptocoins must be saved.

### ✅ 3. Test the Razor Frontend

- The view in `/Views/Home/Index.cshtml` includes a button to call the API.
- Ensure that the interface provides clear feedback to the user indicating whether the update succeeded or failed.

### ✅ 4. Create the Frontend Visualization in `Index.cshtml`

Build a clean and user-friendly visualization directly in the existing Razor view:  
**`/Views/Home/Index.cshtml`**

This page must:

- Display **each cryptocurrency asset** with at least the following fields:
  - ✅ **Name**
  - ✅ **Symbol**
  - ✅ **Current Price**
  - ✅ **Currency**
  - ✅ **Icon**
    - Add the `IconUrl` property to the `CryptoAsset` model
    - Retrieve the image url from CoinGecko
    - Save it to the database
  - ✅ **Last Updated**
    - This value must be converted to the client's local timezone using JavaScript
  - ✅ **Trend**
    - Add a visual indicator showing whether the price has increased, decreased, or stayed the same compared to the previous saved price (e.g., 🔼, 🔽, ➖)
    - Optionally, include the percentage change

- Use the existing "Update Prices" button to trigger the API and refresh the data displayed in the view.

> 🎯 **Goal:** The user should be able to open the page, see the most recent cryptocurrency data clearly presented, and update it with one click.

> 💡 Bonus points for improved UI, animations, or anything that enhances clarity and usability.

### ✅ 5. Add Improvements

- Add validation logic or error handling where needed
- Create unit tests for validator or service logic

---

## ⚙️ How to Run the Project

1. Make sure you have **.NET 6 SDK** installed.
2. Navigate to the backend project folder:
   ```bash
   cd Backend/CryptoPriceTracker.Api
   ```
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Generate your own migrations and update the database:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
5. Run the project:
   ```bash
   dotnet run
   ```
6. Navigate to `http://localhost:5000` to test the Razor page.

---

## 🧪 Testing

You may add unit tests to demonstrate how you validate business logic (e.g., in the `PriceValidator` class).

---

---

## 🛠️ Updated Setup & Run Instructions

To run this solution, ensure you have the **.NET 6 SDK** installed.

### 1. Install Entity Framework Tools
Since this is a .NET 6 project, you must install the compatible version of the EF tool:
```bash
dotnet tool install --global dotnet-ef --version "6.*"
```
*Note: If the command is not found after installation, add the tools to your path: `export PATH="$PATH:$HOME/.dotnet/tools"`*

### 2. Prepare the Database
Navigate to the API project folder and apply the migrations:
```bash
cd CryptoPriceTracker.Api
dotnet ef migrations add AddIconUrl
dotnet ef database update
```

### 3. Run the Project
```bash
dotnet run
```
Navigate to `http://localhost:5000` to view the dashboard.

### 4. Run Unit Tests
A separate test project has been added to validate business logic:
```bash
cd ..
dotnet test
```

---

## 🧠 Architecture & Technical Decisions

### 1. Project Structure
- **Namespaces**: Standardized all project files with `CryptoPriceTracker.Api.*` namespaces for better organization and consistency.
- **Test Project**: Added `CryptoPriceTracker.Tests` using **xUnit** to isolate business logic testing from the main API.

### 2. Service Logic (CoinGecko Integration)
- **Endpoint**: Switched to `/coins/markets` to retrieve names, symbols, current prices, and icon URLs in a single optimized request.
- **User-Agent**: Added a custom User-Agent header to the `HttpClient` as required by the CoinGecko API to prevent request blocking.

### 3. Duplicate Handling
- **Strategy**: Duplicates are prevented by checking if a record already exists for the **same asset** with the **same price** on the **same calendar day**. This ensures the history remains clean while still allowing daily tracking.

### 4. Frontend UI
- **Technology**: Built a custom, responsive dashboard using Vanilla CSS and JavaScript within the Razor view.
- **Timezone**: All "Last Updated" timestamps are converted to the user's local timezone in the browser to ensure accuracy for global users.

---

## 🧾 Deliverables Checklist

- [x] `CryptoPriceService.cs` fetches, saves, and avoids duplicates correctly
- [x] `namespace` declarations are consistent and correct across all files
- [x] `POST /api/crypto/update-prices` endpoint is functional
- [x] `GET /api/crypto/latest-prices` endpoint returns most recent prices
- [x] `IconUrl` property added to `CryptoAsset` model and stored in DB
- [x] Razor page (`Index.cshtml`) includes:
  - [x] Name
  - [x] Symbol
  - [x] Current Price and currency
  - [x] Icon
  - [x] Last Updated (adjusted to client timezone)
  - [x] Trend (up/down indicator and percentage change)
- [x] Button updates data and refreshes the view
- [x] Clear UI feedback after update attempts (success/failure)
- [x] Validation/error handling is implemented where needed
- [x] At least one unit test demonstrates validation logic
- [x] Comments provided for any assumptions or technical decisions

---

Good luck, and thank you for taking the time to complete this challenge!