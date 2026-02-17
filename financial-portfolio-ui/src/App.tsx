// import { useState } from 'react';
import { Dashboard } from './pages/Dashboard';
// import { PortfolioDetails } from './pages/PortfolioDetails';
//import { HooksDemo } from './pages/HooksDemo';

function App() {
 // return <HooksDemo />
 return <Dashboard />
//  const [selectedPortfolioId, setSelectedPortfolioId] = useState<number | null>(null);

//   // If portfolio selected, show details
//   if (selectedPortfolioId) {
//     return (
//       <div>
//         <button 
//           onClick={() => setSelectedPortfolioId(null)}
//           className="fixed top-4 left-4 px-4 py-2 bg-blue-600 text-white rounded-lg z-50"
//         >
//           ← Back to Dashboard
//         </button>
//         <PortfolioDetails portfolioId={selectedPortfolioId} />
//       </div>
//     );
//   }

//   // Otherwise show dashboard
//   return <Dashboard onSelectPortfolio={setSelectedPortfolioId} />;
}

export default App;