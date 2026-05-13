import { useQuery, useMutation, useQueryClient } from "@tanstack/vue-query";
import {
  fetchBacklogs,
  fetchAllBacklogs,
  fetchBacklogById,
  createBacklog,
  updateBacklog,
  deleteBacklog,
  updateBacklogStatus,
  assignBacklogToUser,
} from "../services/backlogApi";

export function useBacklogs(featureId?: string) {
  return useQuery({
    queryKey: ["backlogs", featureId ?? "all"],
    queryFn: async () => {
      if (featureId) {
        return (await fetchBacklogs(featureId)).data;
      }

      return (await fetchAllBacklogs()).data;
    },
    enabled: true,
  });
}

export function useBacklog(backlogId: string) {
  return useQuery({
    queryKey: ["backlog", backlogId],
    queryFn: async () => (await fetchBacklogById(backlogId)).data,
    enabled: !!backlogId,
  });
}

type CreateBacklogPayload = {
  featureId: string;
  data: {
    title: string;
    description: string;
    priority?: string;
    type?: string;
    assignedToUserId?: string | null;
  };
};

type UpdateBacklogPayload = {
  backlogId: string;
  data: {
    title: string;
    description: string;
    priority: string;
    type: string;
    assignedToUserId?: string | null;
  };
};

type UpdateBacklogStatusPayload = {
  backlogId: string;
  status: string;
};

type AssignBacklogPayload = {
  backlogId: string;
  userId: string;
};

const updateBacklogCache = (queryClient: ReturnType<typeof useQueryClient>, updatedBacklog: any) => {
  if (!updatedBacklog?.id) return;

  queryClient.setQueryData(["backlog", updatedBacklog.id], updatedBacklog);
  queryClient.setQueriesData({ queryKey: ["backlogs"] }, (oldData: any) => {
    if (!Array.isArray(oldData)) return oldData;

    return oldData.map((backlog: any) =>
      backlog.id === updatedBacklog.id ? { ...backlog, ...updatedBacklog } : backlog
    );
  });
};

export function useCreateBacklog() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (variables: CreateBacklogPayload) => createBacklog(variables.featureId, variables.data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["backlogs"] });
      queryClient.invalidateQueries({ queryKey: ["backlogs", variables.featureId] });
    },
  });
}

export function useDeleteBacklog() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (backlogId: string) => deleteBacklog(backlogId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["backlogs"] });
    },
  });
}

export function useUpdateBacklog() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (variables: UpdateBacklogPayload) => updateBacklog(variables.backlogId, variables.data),
    onSuccess: (response, variables) => {
      updateBacklogCache(queryClient, response.data);
      return Promise.all([
        queryClient.invalidateQueries({ queryKey: ["backlogs"] }),
        queryClient.invalidateQueries({ queryKey: ["backlog", variables.backlogId] }),
      ]);
    },
  });
}

export function useUpdateBacklogStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (variables: UpdateBacklogStatusPayload) => updateBacklogStatus(variables.backlogId, variables.status),
    onSuccess: (response, variables) => {
      updateBacklogCache(queryClient, response.data);
      return Promise.all([
        queryClient.invalidateQueries({ queryKey: ["backlogs"] }),
        queryClient.invalidateQueries({ queryKey: ["backlog", variables.backlogId] }),
      ]);
    },
  });
}

export function useAssignBacklog() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (variables: AssignBacklogPayload) =>
      assignBacklogToUser(variables.backlogId, variables.userId),
    onSuccess: (response, variables) => {
      updateBacklogCache(queryClient, response.data);
      return Promise.all([
        queryClient.invalidateQueries({ queryKey: ["backlogs"] }),
        queryClient.invalidateQueries({ queryKey: ["backlog", variables.backlogId] }),
      ]);
    },
  });
}
