import * as ChatFlow from './chatflow.models'

export interface ProjectListResponse {
    Message: string;
    Data: ChatFlow.ANAProject[];
}