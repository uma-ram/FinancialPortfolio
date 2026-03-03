import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Dashboard } from './pages/Dashboard';
import { Portfolios } from './pages/Portfolios';
import { PortfolioDetailsPage } from './pages/PortfolioDetailsPage';
import { Transactions } from './pages/Transactions';
import { Analytics } from './pages/Analytics';
import { NotFound } from './pages/NotFound';

function App() {

  return(
     <BrowserRouter>
      <Routes>
        {/* Routes with Layout (includes Navbar) */}
        <Route path="/" element={<Layout />}>
          <Route index element={<Dashboard />} />
          <Route path="portfolios" element={<Portfolios />} />
          <Route path="portfolio/:id" element={<PortfolioDetailsPage />} />
          <Route path="transactions" element={<Transactions />} />
          <Route path="analytics" element={<Analytics />} />
          
          {/* 404 Not Found */}
          <Route path="*" element={<NotFound />} />
        </Route>
      </Routes>
    </BrowserRouter>
  )
}

export default App;