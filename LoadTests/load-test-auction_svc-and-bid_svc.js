
import http from 'k6/http';
import { check, sleep } from 'k6';
import { SharedArray } from 'k6/data';

// Configuration
export const options = {
    stages: [
        { duration: '30s', target: 20 }, // Ramp-up to 20 users over 30 seconds
        { duration: '1m', target: 20 },  // Stay at 20 users for 1 minute
        { duration: '30s', target: 0 },  // Ramp-down to 0 users over 30 seconds
    ],
};

// Base URL of Micro Services API
const BASE_URL_AUCTIONSERVICE = 'https://localhost:7269'; 
const BASE_URL_BIDSERVICE = 'https://localhost:7063'; 

//const BASE_URL_AUCTIONSERVICE = 'http://localhost:5213'; 
//const BASE_URL_BIDSERVICE = 'http://localhost:5144'; 


// Pre-populated data for users and cars
const users = new SharedArray('users', () => [1, 2, 3, 4]); 
const cars = new SharedArray('cars', () => [1, 2]); 

// Helper function to create a new auction
function createAuction() {
    const payload = JSON.stringify({
        carId: cars[Math.floor(Math.random() * cars.length)], // Random car ID
        startDate: new Date().toISOString(),
        endDate: new Date(Date.now() + 3600 * 1000).toISOString(), // 1 hour from now
        auctionType: 'Target', // Can be 'Target', 'BuyNow', or 'Dynamic'
        targetPrice: Math.floor(Math.random() * 1000) + 100, // Random target price between 100 and 1100
        basePrice: Math.floor(Math.random() * 500) + 50, // Random base price between 50 and 550
    });

    const headers = { 'Content-Type': 'application/json' };
    const res = http.post(`${BASE_URL_AUCTIONSERVICE}/auctions/create`, payload, { headers });

    check(res, {
        'Auction created successfully': (r) => r.status === 201,
    });

    if (res.status === 201) {
        const auction = JSON.parse(res.body);
        return auction.auctionId; // Return the created auction ID
    }

    return null;
}

// Helper function to submit a bid
function submitBid(auctionId) {
    const payload = JSON.stringify({
        auctionId: auctionId,
        userId: users[Math.floor(Math.random() * users.length)], // Random user ID
        bidAmount: Math.floor(Math.random() * 1000) + 1, // Random bid amount between 1 and 1000
    });

    const headers = { 'Content-Type': 'application/json' };
    const res = http.post(`${BASE_URL_BIDSERVICE}/bids/create`, payload, { headers });

    check(res, {
        'Bid submitted successfully': (r) => r.status === 201,
    });
}

// Shared state for auctions
let auctionIds = [];

// Main test function
export default function () {
    // Occasionally create a new auction
    if (Math.random() < 0.1) { // 10% chance to create a new auction
        const auctionId = createAuction();
        if (auctionId) {
            auctionIds.push(auctionId); // Add the new auction ID to the list
        }
    }

    // Submit bids for existing auctions
    if (auctionIds.length > 0) {
        const auctionId = auctionIds[Math.floor(Math.random() * auctionIds.length)]; // Random auction ID
        submitBid(auctionId);
    }

    sleep(1); // Pause for 1 second between iterations
}
