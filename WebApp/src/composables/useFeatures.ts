import { useQuery, useMutation, useQueryClient } from "@tanstack/vue-query";
import {
  fetchFeatures,
  fetchFeatureById,
  createFeature,
  deleteFeature,
  updateFeatureStatus,
} from "../services/featureApi";

export function useFeatures(projectId: string) {
  return useQuery({
    queryKey: ["features", projectId],
    queryFn: async () => (await fetchFeatures(projectId)).data,
    enabled: !!projectId,
  });
}

export function useFeature(featureId: string) {
  return useQuery({
    queryKey: ["feature", featureId],
    queryFn: async () => (await fetchFeatureById(featureId)).data,
    enabled: !!featureId,
  });
}

type CreateFeaturePayload = {
  projectId: string;
  data: { name: string; description: string; assignedToUserId?: string | null };
};

type UpdateFeatureStatusPayload = {
  featureId: string;
  status: string;
};

export function useCreateFeature() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (variables: CreateFeaturePayload) => createFeature(variables.projectId, variables.data),
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ["features", variables.projectId] });
    },
  });
}

export function useDeleteFeature() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (featureId: string) => deleteFeature(featureId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["features"] });
    },
  });
}

export function useUpdateFeatureStatus() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (variables: UpdateFeatureStatusPayload) => updateFeatureStatus(variables.featureId, variables.status),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["features"] });
    },
  });
}
