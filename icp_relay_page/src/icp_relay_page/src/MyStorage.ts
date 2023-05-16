import { AuthClientStorage } from "@dfinity/auth-client/lib/cjs/storage";

export class MyStorage implements AuthClientStorage 
{
    name:string = "MyStorage";

    constructor (dbName : string = "MyStorage")
    {
      this.name = dbName;
    }
 
    async get(key: string): Promise<string | null> {
      var val = localStorage.getItem(key);
      if (val == null)
        return JSON.parse (localStorage.getItem(key));
        
      return null;
    } 
 
    async set(key: string, value: string): Promise<void> {
      localStorage.setItem(this.name + ":" + key, JSON.stringify(value));
    }
 
    async remove(key: string): Promise<void> {
       localStorage.removeItem (this.name + ":" + key);
    }
 }