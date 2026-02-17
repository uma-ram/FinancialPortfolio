import { useState, useEffect } from "react";
import { Loader2 } from "lucide-react";

export const HooksDemo = ()=>{
 
    //useState - managing state

    //State is a data that can cahange and triggers re-render
    const [ count, setCount] = useState(0);
    const [name, setName] = useState('');
    const [ items, setItems] = useState<string[]>([]);

    console.log(' Component rendering...');
    console.log('Current count:', count);

    //useEffect - side effects
    // Example 1: Run once on mount (empty dependency array)

    useEffect(()=>{
       console.log('Component MOUNTED (runs once)'); 

        // Cleanup function (runs on unmount)
        return () => {
        console.log('Component UNMOUNTING (cleanup)');
        };
    }, []);// ← Empty array = run once

    // Example 2: Run when count changes
    useEffect(() => {
        console.log(' Count changed to:', count);
    }, [count]); // ← Run when count changes

    // Example 3: Run on every render (no dependency array)
    useEffect(() => {
        console.log('This runs on EVERY render (be careful!)');
    }); // ← No array = runs every render (usually a bug!)

    // Event Handlers
    const handleIncrement = () => {
        // This TRIGGERS a re-render
        setCount(count + 1);
        console.log('After setCount called (but not updated yet!):', count);
        // State updates are ASYNCHRONOUS!
    };

    const handleAddItem = () => {
        // Correct way to update arrays
        setItems([...items, `Item ${items.length + 1}`]);
    };
    return (
        <div className="container mx-auto p-8">
            <h1 className="text-3xl font-bold mb-8">React Hooks Demo</h1>

            {/* Counter Example */}
            <div className="bg-white p-6 rounded-lg shadow-md mb-6">
                <h2 className="text-xl font-bold mb-4">useState Example</h2>
                <p className="mb-4">Count: {count}</p>
                <button
                onClick={handleIncrement}
                className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700"
                >
                Increment
                </button>
            </div>

            {/* Input Example */}
            <div className="bg-white p-6 rounded-lg shadow-md mb-6">
                <h2 className="text-xl font-bold mb-4">Controlled Input</h2>
                <input
                type="text"
                value={name}
                onChange={(e) => setName(e.target.value)}
                className="border px-4 py-2 rounded"
                placeholder="Type your name"
                />
                <p className="mt-4">Hello, {name || 'stranger'}!</p>
            </div>

            {/* List Example */}
            <div className="bg-white p-6 rounded-lg shadow-md">
                <h2 className="text-xl font-bold mb-4">Array State</h2>
                <button
                onClick={handleAddItem}
                className="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700 mb-4"
                >
                Add Item
                </button>
                <ul className="space-y-2">
                {items.map((item, index) => (
                    <li key={index} className="p-2 bg-gray-100 rounded">
                    {item}
                    </li>
                ))}
                </ul>
            </div>
        </div>
    );
};