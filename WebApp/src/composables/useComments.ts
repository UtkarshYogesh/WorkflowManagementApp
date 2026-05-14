import { useMutation, useQuery, useQueryClient } from "@tanstack/vue-query";
import {
  createComment,
  deleteComment,
  fetchComments,
  updateComment,
  type CommentRequest,
  type CommentEntityType,
} from "../services/commentApi";

const commentsKey = (entityType: CommentEntityType, entityId: string) => ["comments", entityType, entityId];

export function useComments(entityId: string, entityType: CommentEntityType) {
  return useQuery({
    queryKey: commentsKey(entityType, entityId),
    queryFn: async () => (await fetchComments(entityId, entityType)).data,
    enabled: !!entityId,
  });
}

export function useCreateComment() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (data: CommentRequest) => createComment(data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: commentsKey(variables.entityType, variables.entityId) });
    },
  });
}

export function useUpdateComment() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ commentId, data }: { commentId: string; data: CommentRequest }) =>
      updateComment(commentId, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: commentsKey(variables.data.entityType, variables.data.entityId) });
    },
  });
}

export function useDeleteComment() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ commentId }: { commentId: string; entityType: CommentEntityType; entityId: string }) =>
      deleteComment(commentId),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: commentsKey(variables.entityType, variables.entityId) });
    },
  });
}
