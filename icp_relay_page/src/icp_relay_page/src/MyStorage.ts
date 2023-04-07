import { AuthClientStorage } from "@dfinity/auth-client/lib/cjs/storage";

export class MyStorage implements AuthClientStorage 
{
    myState : string = "";
 
    async get(key: string): Promise<string | null> {
      this.myState = JSON.parse (localStorage.getItem(key));

      return this.myState;
    } 
 
    async set(key: string, value: string): Promise<void> {
      localStorage.setItem(key, JSON.stringify(value));

      this.myState = value;
    }
 
    async remove(key: string): Promise<void> {
       this.myState = "";
       localStorage.removeItem (key);
    }
 }