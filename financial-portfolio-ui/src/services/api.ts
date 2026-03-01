import axios from "axios";
import type { Portfolio,  PortfolioSummary,  User,  Transaction, Account } from "../types";

//Create axios instance with base URL
const api = axios.create({
    baseURL: "https://localhost:5001/api",
    headers: {
        "Content-Type": 'application/json',
    },
});

//API Service
export const  portfolioApi ={

    //Users
    getUsers : async():Promise<User[]> =>{
        const response = await api.get<User[]>('/users');
        return response.data;
    },

    getUser: async(id: number):Promise<User>=>{
        const response = await api.get<User>(`/users/${id}`);
        return response.data;
    },

    createUser: async(data:{name:string, email:string}):Promise<User>=>{
        const response = await api.post<User>(`/users`,data);
        return response.data;
    },

    //Portfolios
    getUserPortfolios: async(userId: number): Promise<Portfolio[]> =>{
        const response = await api.get<Portfolio[]>(`/portfolios/user/${userId}`);
        return response.data;
    },

    getPortfolio: async (id: number): Promise<Portfolio> => {
        const response = await api.get<Portfolio>(`/portfolios/${id}`);
        return response.data;
    },

    getPortfolioSummary: async (id: number): Promise<PortfolioSummary> => {
        const response = await api.get<PortfolioSummary>(`/portfolios/${id}/summary`);
        return response.data;
    },

    createPortfolio: async (data: {
        name: string;
        description?: string;
        userId: number;
    }): Promise<Portfolio> => {
        const response = await api.post<Portfolio>(`/portfolios`, data);
        return response.data;
    },

    //Transaction
    createTransaction: async (data:{
        accountId: number;
        transactionType: string;
        symbol?: string;
        quantity: number;
        price: number;
        notes?: string;
    }): Promise<Transaction> =>{
        const resposne = await api.post<Transaction>(`/transactions`, data);
        return resposne.data;
    },

    getPortfolioAccounts: async (id:number): Promise<Account[]> =>{    
        const response = await api.get<Account[]>(`/accounts/portfolio/${id}`);
        return response.data;
    },

    getAccountTransactions: async (accountId : number): Promise<Transaction[]> =>{
        const response = await api.get<Transaction[]>(`/transactions/account/${accountId}`);
        //console.log(response.data.transactions);
        return response.data; //.transactions;
    },


};

export default api;