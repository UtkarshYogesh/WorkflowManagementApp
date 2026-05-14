import api from "./api";

export enum CommentEntityType {
  Project = 0,
  Feature = 1,
  BacklogItem = 2,
  Task = 3,
}

export type CommentRequest = {
  entityId: string;
  entityType: CommentEntityType;
  body: string;
  mentionUserIds: string[];
};

export type CommentResponse = {
  id: string;
  entityId: string;
  entityType: CommentEntityType;
  body: string;
  createdAt: string;
  updatedAt?: string | null;
  createdByUserId: string;
  mentionedUserIds: string[];
};

export const fetchComments = (entityId: string, entityType: CommentEntityType) =>
  api.get<CommentResponse[]>("/comments", {
    params: { entityId, entityType },
  });

export const createComment = (data: CommentRequest) => api.post<CommentResponse>("/comments", data);

export const updateComment = (commentId: string, data: CommentRequest) =>
  api.put<CommentResponse>(`/comments/${commentId}`, data);

export const deleteComment = (commentId: string) => api.delete(`/comments/${commentId}`);
