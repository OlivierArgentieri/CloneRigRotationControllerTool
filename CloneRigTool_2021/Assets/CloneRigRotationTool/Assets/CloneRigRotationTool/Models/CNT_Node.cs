namespace CloneRigRotationTool.Assets.CloneRigRotationTool.Models
{
    public class NodeCNT
    {
        public float rot_x;
        public float rot_y;
        public float rot_z;
        public float rot_w;
        public string name;
        public int childNumber;
        public NodeCNT[] next = null;
        public NodeCNT(string _name, float _rot_x, float _rot_y, float _rot_z, float _rot_w, int _childNumber, NodeCNT[] _next = null)
        {
            rot_x = _rot_x;
            rot_y = _rot_y;
            rot_z = _rot_z;
            rot_w = _rot_w;
            name = _name;
            childNumber = _childNumber;
            next = _next;
        }
    }
}