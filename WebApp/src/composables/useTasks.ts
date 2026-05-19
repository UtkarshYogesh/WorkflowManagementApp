import { useQuery, useMutation, useQueryClient } from "@tanstack/vue-query";
import {
  fetchTasks,
  fetchAllTasks,
  createTask,
  updateTask,
  deleteTask,
  updateTaskStatus,
  fetchTaskById,
  assignTaskToUser,
} from "../services/taskApi";

export function useTasks(backlogId?: string) {
  return useQuery({
    queryKey: ["tasks", backlogId ?? "all"],
    queryFn: async () => {
      if (backlogId) {
        return (await fetchTasks(backlogId)).data;
      }

      return (await fetchAllTasks()).data;
    },
    enabled: true,
  });
}

export function useCreateTask() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ backlogId, data }: { backlogId: string; data: any }) => createTask(backlogId, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["tasks", variables.backlogId] });
    },
  });
}

export function useDeleteTask() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (taskId: string) => deleteTask(taskId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["tasks"] });
    },
  });
}

export function useUpdateTaskStatus() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ taskId, status }: { taskId: string; status: string }) => updateTaskStatus(taskId, status),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["tasks"] });
      queryClient.invalidateQueries({ queryKey: ["task", variables.taskId] });
    },
  });
}

export function useUpdateTask() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ taskId, data }: { taskId: string; data: any }) => updateTask(taskId, data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["tasks"] });
      queryClient.invalidateQueries({ queryKey: ["task", variables.taskId] });
    },
  });
}

export function useAssignTask() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({ taskId, userId }: { taskId: string; userId: string }) =>
      assignTaskToUser(taskId, userId),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["tasks"] });
      queryClient.invalidateQueries({ queryKey: ["task", variables.taskId] });
    },
  });
}

export function useTask(taskId: string) {
  return useQuery({
    queryKey: ["task", taskId],
    queryFn: async () => (await fetchTaskById(taskId)).data,
    enabled: !!taskId,
  });
}
