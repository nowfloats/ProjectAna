import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ChatServerConnection, ChatBotProject } from '../models/app.models';

@Injectable()
export class DataService {

	constructor(private http: HttpClient) { }
	connection: ChatServerConnection;

	getUserAccounts() {

	}

	createUserAccount() {

	}


}