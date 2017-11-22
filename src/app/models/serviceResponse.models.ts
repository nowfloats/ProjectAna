import { ChatBotProject } from './app.models'

export interface ProjectListResponse {
    Message: string;
    Data: ChatBotProject[];
}
