import { Injectable } from '@angular/core';
import { Http, RequestOptionsArgs, Headers } from '@angular/http';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import * as ChatFlowModels from '../models/chatflow.models'
import * as ServiceResponseModels from '../models/serviceResponse.models'

@Injectable()
export class ChatFlowService {
    constructor(private http: Http) { }

    //Chat server base URL
    private baseUrl: string = "http://ria.nowfloatsdev.com/chat4/"; //"http://localhost:2646/";

    private auth: string = 'Basic bm93ZmxvYXRzOm5vd2Zsb2F0czEyMw==';
    private loadProjectsAPI: string = this.baseUrl + "api/Project/List";
    private saveProjectsAPI: string = this.baseUrl + "api/Project/Save";
    private saveChatFlowPackAPI: string = this.baseUrl + "api/Conversation/SaveChatFlow";
    private fetchChatFlowPackAPI: string = this.baseUrl + "api/Conversation/FetchChatFlowPack?projectId={projectId}";

    private getHttpOptions() {
        var headers = new Headers();
        headers.set('Authorization', this.auth)
        return {
            headers: headers,
        };
    }

    loadProjectsList() {
        return this.http.get(this.loadProjectsAPI, this.getHttpOptions()).map(res =>
            res.json() as ServiceResponseModels.ProjectListResponse);
    }

    fetchChatFlowPack(projectId: string) {
        return this.http.get(this.fetchChatFlowPackAPI.replace("{projectId}", projectId), this.getHttpOptions()).map(res =>
            res.json() as ChatFlowModels.ChatFlowPack);
    }
}